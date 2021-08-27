using Xunit;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased
{
    public class Properties
    {
        [Fact]
        public void Properties_with_known_types()
        {
            "properties/known_types.cs"
                .Compiles()
                .WithDto("Dto")
                    .WithProperty("A", Known(KnownType.Int32))
                    .WithProperty("B", Known(KnownType.Uint64))
                    .WithProperty("C", Known(KnownType.String))
                    .WithProperty("D", Known(KnownType.Boolean))
                    .WithProperty("E", Known(KnownType.Uri))
                    .WithProperty("F", Known(KnownType.DateOnly))
                    .WithProperty("G", Known(KnownType.TimeOnly))
                    .WithProperty("H", Known(KnownType.Date))
                    .WithProperty("I", Known(KnownType.Time))
                    .WithProperty("J", Known(KnownType.DateTime))
                    .WithProperty("K", Known(KnownType.DateTimeOffset))
                    .WithProperty("L", Known(KnownType.Guid))
                    .WithProperty("M", Known(KnownType.Float))
                    .WithProperty("N", Known(KnownType.Double))
                    .WithProperty("O", Known(KnownType.TimeSpan))
                    .WithProperty("P", Known(KnownType.CommandResult));
        }

        [Fact]
        public void Properties_with_composite_types()
        {
            "properties/composite_types.cs"
                .Compiles()
                .WithDto("Dto")
                    .WithProperty("A", Array(Known(KnownType.Int32)))
                    .WithProperty("B", Array(Known(KnownType.Int32)))
                    .WithProperty("C", Array(Known(KnownType.Int32)))
                    .WithProperty("D", Array(Known(KnownType.Int32)))
                    .WithProperty("E", Map(Known(KnownType.Int32), Known(KnownType.String)))
                    .WithProperty("F", Map(Known(KnownType.Int32), Known(KnownType.String)))
                    .WithProperty("G", Map(Known(KnownType.Int32), Known(KnownType.String)));
        }
    }
}
