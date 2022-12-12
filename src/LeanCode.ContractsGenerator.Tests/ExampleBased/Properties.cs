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
                .WithProperty("H", Known(KnownType.DateTimeOffset))
                .WithProperty("I", Known(KnownType.Guid))
                .WithProperty("J", Known(KnownType.Float32))
                .WithProperty("K", Known(KnownType.Float64))
                .WithProperty("L", Known(KnownType.TimeSpan))
                .WithProperty("M", Known(KnownType.CommandResult));
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

    [Fact]
    public void Properties_inside_struct_types()
    {
        "properties/struct.cs"
            .Compiles()
            .WithDto("DTO")
                .WithProperty("A", Known(KnownType.Int32));
    }

    [Fact]
    public void Properties_with_struct_types()
    {
        "properties/inner_struct.cs"
            .Compiles()
            .WithDto("DTO")
                .WithProperty("A", TypeRefExtensions.Internal("InnerDTO"))
                .WithProperty("B", TypeRefExtensions.Internal("InnerDTO").Nullable());
    }
}
