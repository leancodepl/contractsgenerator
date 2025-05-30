using LeanCode.ContractsGenerator.Analyzers;
using Xunit;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers;

public class InvalidTypes
{
    [Fact]
    public void Invalid_types_are_reported()
    {
        "analyzers/invalid_types.cs"
            .AnalyzeFails()
            .WithErrorNumber(1)
            .WithError(
                AnalyzerCodes.UnsupportedType,
                "Dto.Wrong1",
                messagePattern: ".+Use `DateTimeOffset` with zero offset instead.+"
            );
    }

    [Fact]
    public void Invalid_DateTime_type_errors_can_be_suppressed()
    {
        "analyzers/invalid_types.cs"
            .Compiles(new(AllowDateTime: true))
            .WithDto("Dto")
            .WithProperty("Wrong1", Known(KnownType.DateTime));
    }
}
