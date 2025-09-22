using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace LeanCode.ContractsGenerator.Compilation;

public static class ContractsCompiler
{
    public static readonly FrozenSet<string> DefaultAssemblyNames = FrozenSet.Create(
        StringComparer.InvariantCultureIgnoreCase,
        "System.Collections",
        "System.Linq",
        "System.Net.Http",
        "System.Runtime",
        "System.Runtime.Extensions"
    );

    public static readonly FrozenSet<string> LeanCodeAssemblyNames = FrozenSet.Create(
        StringComparer.InvariantCultureIgnoreCase,
        "LeanCode.Contracts"
    );

    private static bool IsWantedDefaultAssembly(CompilationLibrary cl) => DefaultAssemblyNames.Contains(cl.Name);

    private static bool IsWantedLeanCodeAssembly(CompilationLibrary cl) => LeanCodeAssemblyNames.Contains(cl.Name);

    private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

    private static readonly AppBaseCompilationAssemblyResolver Resolver = new(
        Path.GetDirectoryName(ExecutingAssembly.Location)!
    );

    public static readonly ImmutableList<PortableExecutableReference> DefaultAssemblies =
    [
        .. DependencyContext
            .Load(ExecutingAssembly)!
            .CompileLibraries.Where(cl => IsWantedDefaultAssembly(cl) || IsWantedLeanCodeAssembly(cl))
            .SelectMany(cl => cl.ResolveReferencePaths(Resolver))
            .Select(path => MetadataReference.CreateFromFile(path)),
    ];

    public static Task<(CompiledContracts Compiled, List<Export> External)> CompileProjectsAsync(
        IEnumerable<string> projectPaths
    ) => CompileProjectsAsync(projectPaths, ImmutableDictionary<string, string>.Empty);

    public static async Task<(CompiledContracts Compiled, List<Export> External)> CompileProjectsAsync(
        IEnumerable<string> projectPaths,
        ImmutableDictionary<string, string> properties
    )
    {
        using var loader = new ProjectLoader(properties);
        await loader.LoadProjectsAsync(projectPaths);
        var compilations = await loader.CompileAsync();
        var compiledContracts = Compile(compilations, compilations.First().AssemblyName ?? string.Empty);
        var externalContracts = TryLoadEmbeddedContracts(compilations);
        return (compiledContracts, externalContracts);
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

    public static async Task<CompiledContracts> CompileGlobAsync(Matcher matcher, DirectoryInfo directory)
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
        return CompileTrees([contractTree], name);
    }

    private static CompiledContracts CompileTrees(List<SyntaxTree> trees, string name)
    {
        var compilation = CSharpCompilation
            .Create("LeanCode.ContractsGenerator")
            .WithOptions(PrepareCompilationOptions())
            .AddReferences(DefaultAssemblies)
            .AddSyntaxTrees(trees);

        return Compile(compilation, name);
    }

    private static CSharpCompilationOptions PrepareCompilationOptions() =>
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithConcurrentBuild(true)
            .WithAllowUnsafe(false)
            .WithNullableContextOptions(NullableContextOptions.Annotations)
            .WithPlatform(Platform.AnyCpu);

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "?",
        "CA1031",
        Justification = "Failure to extract embedded contracts from some assemblies should not interrupt the operation."
    )]
    private static List<Export> TryLoadEmbeddedContracts(IReadOnlyCollection<CSharpCompilation> compilations)
    {
        var exports = new List<Export>();
        var processed = new HashSet<string>();

        foreach (var compilation in compilations)
        {
            var externalReferences = compilation
                .ExternalReferences.OfType<PortableExecutableReference>()
                .Where(per => !string.IsNullOrEmpty(per.FilePath) && File.Exists(per.FilePath))
                .Select(per => per.FilePath!)
                .Distinct()
                .ToList();

            using var context = new MetadataLoadContext(new PathAssemblyResolver(externalReferences));

            foreach (var assemblyPath in externalReferences.Where(p => !processed.Contains(p)))
            {
                try
                {
                    var assembly = context.LoadFromAssemblyPath(assemblyPath);
                    using var stream = assembly.GetManifestResourceStream("LeanCode.Contracts.pb");

                    if (stream is not null)
                    {
                        exports.Add(Export.Parser.ParseFrom(stream));
                    }

                    processed.Add(assemblyPath);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Failed to search assembly at {assemblyPath} for embedded contracts.");
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }

        return exports;
    }

    private static CompiledContracts Compile(CSharpCompilation compilation, string name) =>
        Compile([compilation], name);

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
