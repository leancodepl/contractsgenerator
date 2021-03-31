using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using LeanCode.CQRS;
using LeanCode.CQRS.Security;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LeanCode.ContractsGeneratorV2
{
    internal class ContractsCompiler
    {
        private static readonly string ObjectAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
        private static readonly List<MetadataReference> DefaultAssemblies = new()
        {
            MetadataReference.CreateFromFile(typeof(IQuery<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICustomAuthorizer<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Date).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(ObjectAssemblyPath, "mscorlib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(ObjectAssemblyPath, "System.dll")),
            MetadataReference.CreateFromFile(Path.Combine(ObjectAssemblyPath, "System.Core.dll")),
            MetadataReference.CreateFromFile(Path.Combine(ObjectAssemblyPath, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(Path.Combine(ObjectAssemblyPath, "System.Private.Uri.dll")),
        };

        private readonly string rootPath;

        public ContractsCompiler(string rootPath)
        {
            this.rootPath = rootPath;

        }

        public Contracts Compile()
        {
            var trees = new List<SyntaxTree>();

            var fileRoot = new DirectoryInfo(rootPath);
            var contracts = fileRoot.GetFiles("*.cs", SearchOption.AllDirectories);

            foreach (var contract in contracts)
            {
                using var fileReader = new StreamReader(contract.OpenRead());
                var contractText = fileReader.ReadToEnd();
                var contractTree = CSharpSyntaxTree.ParseText(contractText, path: contract.FullName);

                trees.Add(contractTree);
            }

            var compilation = CSharpCompilation.Create("LeanCode.ContractsGenerator")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithConcurrentBuild(true)
                    .WithAllowUnsafe(false)
                    .WithNullableContextOptions(NullableContextOptions.Annotations)
                    .WithPlatform(Platform.AnyCpu))
                .AddReferences(DefaultAssemblies)
                .AddSyntaxTrees(trees);

            var diags = compilation.GetDiagnostics();
            if (diags.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                var errors = diags.Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();
                throw new CompilationFailedException(errors);
            }

            return new(compilation, trees);
        }
    }

    public class CompilationFailedException : Exception
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public CompilationFailedException(ImmutableArray<Diagnostic> diagnostics)
            : base("Contracts compilation failed.")
        {
            Diagnostics = diagnostics;
        }
    }

    public class Contracts
    {
        public CSharpCompilation Compilation { get; private init; }
        public IReadOnlyList<SyntaxTree> Trees { get; private init; }

        public INamedTypeSymbol QueryType { get; }
        public INamedTypeSymbol CommandType { get; }

        public INamedTypeSymbol AuthorizeWhenAttribute { get; }
        public INamedTypeSymbol AuthorizeWhenHasAnyOfAttribute { get; }
        public INamedTypeSymbol QueryCacheAttribute { get; }
        public INamedTypeSymbol Attribute { get; }
        public INamedTypeSymbol AttributeUsageAttribute { get; }

        public Contracts(CSharpCompilation compilation, List<SyntaxTree> trees)
        {
            Compilation = compilation;
            Trees = trees;

            QueryType = Compilation.GetTypeByMetadataName(typeof(IRemoteQuery<>).FullName).ConstructUnboundGenericType();
            CommandType = Compilation.GetTypeByMetadataName(typeof(IRemoteCommand).FullName);

            AuthorizeWhenAttribute = Compilation.GetTypeByMetadataName(typeof(AuthorizeWhenAttribute).FullName);
            AuthorizeWhenHasAnyOfAttribute = Compilation.GetTypeByMetadataName(typeof(AuthorizeWhenHasAnyOfAttribute).FullName);
            QueryCacheAttribute = Compilation.GetTypeByMetadataName(typeof(QueryCacheAttribute).FullName);
            Attribute = Compilation.GetTypeByMetadataName(typeof(Attribute).FullName);
            AttributeUsageAttribute = Compilation.GetTypeByMetadataName(typeof(AttributeUsageAttribute).FullName);
        }

        public IEnumerable<INamedTypeSymbol> ListAllTypes()
        {
            return Trees.SelectMany(t =>
            {
                var model = Compilation.GetSemanticModel(t);
                var root = t.GetRoot();
                var symbols = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
                return symbols
                    .Select(s => model.GetDeclaredSymbol(s))
                    .OfType<INamedTypeSymbol>();
            });
        }
    }
}
