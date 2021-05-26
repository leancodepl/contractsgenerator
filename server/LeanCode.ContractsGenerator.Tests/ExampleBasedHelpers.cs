using System.IO;

namespace LeanCode.ContractsGenerator.Tests
{
    public static class ExampleBasedHelpers
    {
        public static Export Compiles(this string path)
        {
            var code = File.ReadAllText(Path.Join("examples", path));
            var compiled = ContractsCompiler.CompileCode(code);
            return new ContractsGenerator(compiled).Generate(string.Empty);
        }
    }
}
