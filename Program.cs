using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeanCode.ContractsGeneratorV2
{
    class Program
    {
        static void Main(string[] args)
        {
            var contracts = new ContractsCompiler("./ExampleContracts").Compile();
            var gen = new ContractsGenerator(contracts);
            var generated = gen.Generate();
            var export = new Export("ExampleContracts", generated);

            var opts = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                    new AttributeArgumentConverter(),
                    new GenericArgumentConverter(),
                    new TypeRefConverter(),
                    new ErrorCodeConverter(),
                    new EnumStatementConverter(),
                    new StatementConverter(),
                },
            };

            var result = JsonSerializer.Serialize(export, opts);
            System.Console.WriteLine(result);
        }
    }
}
