using Xunit;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased;

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
                .WithProperty("H", Known(KnownType.DateTime))
                .WithProperty("I", Known(KnownType.DateTimeOffset))
                .WithProperty("J", Known(KnownType.Guid))
                .WithProperty("K", Known(KnownType.Float32))
                .WithProperty("L", Known(KnownType.Float64))
                .WithProperty("M", Known(KnownType.TimeSpan))
                .WithProperty("N", Known(KnownType.CommandResult));
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

    [Fact]
    public void Properties_with_binary_types()
    {
        "properties/binary.cs"
            .Compiles()
            .WithDto("Dto")
                .WithProperty("A", Known(KnownType.Binary))
                .WithProperty("B", Known(KnownType.Binary).Nullable())
                .WithProperty("C", Array(Known(KnownType.Binary)))
                .WithProperty("D", Map(Known(KnownType.Binary), Known(KnownType.Binary)));
    }
}
