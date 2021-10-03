using System.IO;
using System.Linq;
using LeanCode.ContractsGenerator.Compilation;
using Microsoft.Extensions.FileSystemGlobbing;

namespace LeanCode.ContractsGenerator.Tests
{
    public static class ExampleBasedHelpers
    {
        public static AssertedExport Compiles(this string path)
        {
            var code = File.ReadAllText(Path.Join("examples", path));
            var compiled = ContractsCompiler.CompileCode(code, "test");
            return new(new ContractsGenerator.Generation.ContractsGenerator(compiled).Generate());
        }

        public static AssertedExport ProjectCompiles(this string path)
        {
            return ProjectsCompile(path);
        }

        public static AssertedExport ProjectsCompile(params string[] paths)
        {
            var projectPaths = paths.Select(p => Path.Join("examples", p));
            // HACK: The sync execution results in much cleaner tests
            var compiled = ContractsCompiler.CompileProjectsAsync(projectPaths).Result;
            return new(new ContractsGenerator.Generation.ContractsGenerator(compiled).Generate());
        }

        public static AssertedExport GlobCompiles(string[] includes, string[] excludes)
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(includes);
            matcher.AddExcludePatterns(excludes);
            var di = new DirectoryInfo("examples");
            // HACK: The sync execution results in much cleaner tests
            var compiled = ContractsCompiler.CompileGlobAsync(matcher, di).Result;
            return new(new ContractsGenerator.Generation.ContractsGenerator(compiled).Generate());
        }
    }
}
