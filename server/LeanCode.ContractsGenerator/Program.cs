using System.IO;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace LeanCode.ContractsGenerator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var solutionPath = Path.GetFullPath(args[0]);
            var projectPath = Path.GetFullPath(args[1]);
            await Write(solutionPath, projectPath);
            var export = Read();
            WriteJson(export);
        }

        private static async Task Write(string solutionPath, string projectPath)
        {
            var contracts = await ContractsCompiler.CompileProjectAsync(solutionPath, projectPath);
            var generated = new ContractsGenerator(contracts).Generate();
            using var outputStream = File.OpenWrite("./example.pb");
            using var codedOutput = new CodedOutputStream(outputStream, true);
            generated.WriteTo(codedOutput);
        }

        private static Export Read()
        {
            using var inputStream = File.OpenRead("./example.pb");
            return Export.Parser.ParseFrom(inputStream);
        }

        private static void WriteJson(Export export)
        {
            using var writer = File.CreateText("./example.json");
            writer.Write(export.ToString());
        }
    }
}
