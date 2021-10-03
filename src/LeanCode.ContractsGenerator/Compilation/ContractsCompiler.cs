using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace LeanCode.ContractsGenerator.Compilation
{
    public static class ContractsCompiler
    {
        private static readonly ImmutableList<string> ImplicitGlobalUsings = ImmutableList.CreateRange(new[]
        {
            "System",
            "System.Collections.Generic",
            "System.IO",
            "System.Linq",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
        });

        public static readonly ImmutableHashSet<string> ReferenceAssemblyNames = ImmutableHashSet.CreateRange(new[]
        {
            "System.Collections.Reference",
            "System.Linq.Reference",
            "System.Net.Http.Reference",
            "System.Runtime.Reference",
            "System.Runtime.Extensions.Reference",
        });

        public static readonly ImmutableHashSet<string> LeanCodeAssemblyNames = ImmutableHashSet.CreateRange(new[]
        {
            "LeanCode.CQRS",
            "LeanCode.CQRS.Security",
            "LeanCode.Time",
        });

        private static bool IsWantedReferenceAssembly(CompilationLibrary cl)
        {
            return cl.Type == "referenceassembly"
                && ReferenceAssemblyNames.Contains(cl.Name, StringComparer.InvariantCultureIgnoreCase);
        }

        private static bool IsWantedLeanCodeAssembly(CompilationLibrary cl)
        {
            return LeanCodeAssemblyNames.Contains(cl.Name, StringComparer.InvariantCultureIgnoreCase);
        }

        public static readonly ImmutableList<PortableExecutableReference> DefaultAssemblies = DependencyContext.Default.CompileLibraries
            .Where(cl => IsWantedReferenceAssembly(cl) || IsWantedLeanCodeAssembly(cl))
            .SelectMany(cl => cl.ResolveReferencePaths())
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToImmutableList();

        public static async Task<CompiledContracts> CompileProjectsAsync(IEnumerable<string> projectPaths)
        {
            using var loader = new ProjectLoader(PrepareCompilationOptions());
            await loader.LoadProjectsAsync(projectPaths);
            var compilations = await loader.CompileAsync();
            return Compile(compilations, compilations.First().AssemblyName ?? string.Empty);
        }

        public static async Task<CompiledContracts> CompilePathAsync(string rootPath)
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

        public static async Task<CompiledContracts> CompileGlobAsync(
            Matcher matcher,
            DirectoryInfo directory)
        {
            var dir = new DirectoryInfoWrapper(directory);
            var result = matcher.Execute(dir);
            var trees = new List<SyntaxTree>();
            foreach (var f in result.Files)
            {
                var fp = dir.GetFile(f.Path).FullName;
                var content = await File.ReadAllTextAsync(fp);
                trees.Add(CSharpSyntaxTree.ParseText(content, new(LanguageVersion.Preview)));
            }

            return CompileTrees(trees, directory.FullName);
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
                .WithUsings(ImplicitGlobalUsings)
                .WithConcurrentBuild(true)
                .WithAllowUnsafe(false)
                .WithNullableContextOptions(NullableContextOptions.Annotations)
                .WithPlatform(Platform.AnyCpu);
        }

        private static CompiledContracts Compile(CSharpCompilation compilation, string name)
        {
            return Compile(new List<CSharpCompilation> { compilation }, name);
        }

        private static CompiledContracts Compile(IReadOnlyCollection<CSharpCompilation> compilations, string name)
        {
            foreach (var compilation in compilations)
            {
                var diags = compilation.GetDiagnostics();
                if (diags.Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    var errors = diags.Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();
                    throw new CompilationFailedException(errors);
                }
            }

            return new(compilations, name);
        }
    }
}
