using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            var export = new Export("ExampleContracts", generated, ListKnownGroups(generated).ToImmutableList());

            var opts = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                    new ValueRefConverter(),
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

        private static IEnumerable<ErrorCode.Group> ListKnownGroups(IEnumerable<Statement> statements)
        {
            return statements
                .OfType<Statement.TypeStatement.CommandStatement>()
                .SelectMany(c => ListGroups(c.ErrorCodes));

            static IEnumerable<ErrorCode.Group> ListGroups(IEnumerable<ErrorCode> gs)
            {
                foreach (var g in gs.OfType<ErrorCode.Group>())
                {
                    yield return g;

                    foreach (var i in ListGroups(g.InnerCodes))
                    {
                        yield return i;
                    }
                }
            }
        }
    }
}
