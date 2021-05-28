using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Buildalyzer.Workspaces;
using LeanCode.CQRS;
using LeanCode.CQRS.Security;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LeanCode.ContractsGenerator
{
    public static class ContractsCompiler
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

        private static readonly IReadOnlySet<string> AllowedPackaged = new HashSet<string>
        {
            "LeanCode.CQRS",
            "LeanCode.Time",
        };

        public static async Task<CompiledContracts> CompileProjectAsync(string solutionPath, string projectPath)
        {
            var manager = new AnalyzerManager(solutionPath);
            var project = manager.GetProject(projectPath);
            VerifyProject(project);
            var compilation = await CompileProjectAsync(project);
            return Compile(compilation, compilation.AssemblyName ?? string.Empty);

            static void VerifyProject(IProjectAnalyzer project)
            {
                foreach (var package in project.ProjectFile.PackageReferences)
                {
                    if (!AllowedPackaged.Contains(package.Name))
                    {
                        throw new InvalidProjectException($"The project references package {package.Name} that is not allowed.");
                    }
                }
            }

            static async Task<CSharpCompilation> CompileProjectAsync(IProjectAnalyzer project)
            {
                var id = ProjectId.CreateFromSerialized(project.ProjectGuid);
                var ws = project.GetWorkspace(true);
                var proj = ws.CurrentSolution.GetProject(id)?.WithCompilationOptions(PrepareCompilationOptions());
                if (proj is null)
                {
                    throw new InvalidProjectException("Cannot compile project - the project is not in the solution.");
                }

                var compilation = await proj.GetCompilationAsync();
                return (CSharpCompilation)compilation;
            }
        }

        public static async Task<CompiledContracts> CompilePath(string rootPath)
        {
            var trees = new List<SyntaxTree>();

            var fileRoot = new DirectoryInfo(rootPath);
            var contracts = fileRoot.GetFiles("*.cs", SearchOption.AllDirectories);

            foreach (var contract in contracts)
            {
                using var fileReader = new StreamReader(contract.OpenRead());
                var contractText = await fileReader.ReadToEndAsync();
                var contractTree = CSharpSyntaxTree.ParseText(contractText, path: contract.FullName);

                trees.Add(contractTree);
            }

            return CompileTrees(trees, rootPath);
        }

        public static async Task<CompiledContracts> CompileFileAsync(string filename)
        {
            var content = await File.ReadAllTextAsync(filename);
            return CompileCode(content, filename);
        }

        public static CompiledContracts CompileCode(string contractText, string name)
        {
            var contractTree = CSharpSyntaxTree.ParseText(contractText);
            return CompileTrees(new() { contractTree }, name);
        }

        private static CompiledContracts CompileTrees(List<SyntaxTree> trees, string name)
        {
            var compilation = CSharpCompilation.Create("LeanCode.ContractsGenerator")
                .WithOptions(PrepareCompilationOptions())
                .AddReferences(DefaultAssemblies)
                .AddSyntaxTrees(trees);

            return Compile(compilation, name);
        }

        private static CSharpCompilationOptions PrepareCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithConcurrentBuild(true)
                .WithAllowUnsafe(false)
                .WithNullableContextOptions(NullableContextOptions.Annotations)
                .WithPlatform(Platform.AnyCpu);
        }

        private static CompiledContracts Compile(CSharpCompilation compilation, string name)
        {
            var diags = compilation.GetDiagnostics();
            if (diags.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                var errors = diags.Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();
                throw new CompilationFailedException(errors);
            }

            return new(compilation, name);
        }
    }

    public class InvalidProjectException : Exception
    {
        public InvalidProjectException(string msg)
            : base(msg)
        { }
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
}
