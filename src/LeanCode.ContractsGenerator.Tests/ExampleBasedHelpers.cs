using System.IO;

namespace LeanCode.ContractsGenerator.Tests
{
    public static class ExampleBasedHelpers
    {
        public static AssertedExport Compiles(this string path)
        {
            var code = File.ReadAllText(Path.Join("examples", path));
            var compiled = ContractsCompiler.CompileCode(code, "test");
            return new(new ContractsGenerator(compiled).Generate());
        }

        public static AssertedExport ProjectCompiles(this string path)
        {
            var projectPath = Path.Join("examples", path);
            // HACK: The sync execution results in much cleaner tests
            var compiled = ContractsCompiler.CompileProjectAsync(projectPath).Result;
            return new(new ContractsGenerator(compiled).Generate());
        }
    }
}