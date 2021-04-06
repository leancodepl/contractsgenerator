using System.IO;
using System.Text;
using Google.Protobuf;

namespace LeanCode.ContractsGeneratorV2
{
    class Program
    {
        static void Main(string[] args)
        {
            var fullPath = Path.GetFullPath(args.Length > 1 ? args[1] : "./ExampleContracts");
            Write(fullPath);
            var export = Read();
            WriteJson(export);
        }

        private static void Write(string path)
        {
            var contracts = new ContractsCompiler(path).Compile();
            var gen = new ContractsGenerator(contracts);
            var generated = gen.Generate(path);
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
