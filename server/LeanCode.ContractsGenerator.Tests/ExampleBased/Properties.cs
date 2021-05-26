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
                    .WithProperty("F", Known(KnownType.Date))
                    .WithProperty("G", Known(KnownType.Time))
                    .WithProperty("H", Known(KnownType.DateTime))
                    .WithProperty("I", Known(KnownType.DateTimeOffset))
                    .WithProperty("J", Known(KnownType.Guid))
                    .WithProperty("K", Known(KnownType.Float))
                    .WithProperty("L", Known(KnownType.Double));
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
