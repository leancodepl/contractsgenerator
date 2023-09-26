using LeanCode.ContractsGenerator.Compilation;
using Microsoft.Extensions.FileSystemGlobbing;

namespace LeanCode.ContractsGenerator.Tests;

public static class ExampleBasedHelpers
{
    public static AssertedExport Compiles(this string path)
    {
        var code = File.ReadAllText(Path.Join("examples", path));
        var compiled = ContractsCompiler.CompileCode(code, "test");
        return new(new ContractsGenerator.Generation.ContractsGenerator(compiled).Generate());
    }

    public static AssertedErrors AnalyzeFails(this string path)
    {
        var code = File.ReadAllText(Path.Join("examples", path));
        var compiled = ContractsCompiler.CompileCode(code, "test");
        var ex = Xunit.Assert.Throws<AnalyzeFailedException>(
            () => new ContractsGenerator.Generation.ContractsGenerator(compiled).Generate()
        );
        return new(ex.Errors);
    }

    public static AssertedExport ProjectCompiles(this string path)
    {
        // See `/examples/Directory.Build.targets` for an explanation why we need to set the variable
        Environment.SetEnvironmentVariable("UseTestBuildOfContracts", "true");

        return ProjectsCompile(path);
    }

    public static AssertedExport ProjectsCompile(params string[] paths)
    {
        var projectPaths = paths.Select(p => Path.Join("examples", p));
        // HACK: The sync execution results in much cleaner tests
        var (compiled, external) = ContractsCompiler
            .CompileProjectsAsync(projectPaths)
            .GetAwaiter()
            .GetResult();
        return new(
            new ContractsGenerator.Generation.ContractsGenerator(compiled).Generate(external, false)
        );
    }

    public static AssertedExport GlobCompiles(string[] includes, string[] excludes)
    {
        var matcher = new Matcher();
        matcher.AddIncludePatterns(includes);
        matcher.AddExcludePatterns(excludes);
        var di = new DirectoryInfo("examples");
        // HACK: The sync execution results in much cleaner tests
        var compiled = ContractsCompiler.CompileGlobAsync(matcher, di).GetAwaiter().GetResult();
        return new(new ContractsGenerator.Generation.ContractsGenerator(compiled).Generate());
    }
}
