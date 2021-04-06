using System.IO;

namespace LeanCode.ContractsGeneratorV2
{
    class Program
    {
        static void Main(string[] args)
        {
            var contracts = new ContractsCompiler("./ExampleContracts").Compile();
            var gen = new ContractsGenerator(contracts);
            var generated = gen.Generate("./ExampleContracts");

            using var outputStream = File.OpenWrite("./example.contracts");
            using var codedOutput = new Google.Protobuf.CodedOutputStream(outputStream, true);
            generated.WriteTo(codedOutput);
        }
    }
}
