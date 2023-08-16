using LeanCode.ContractsGenerator.Analyzers;
using Xunit;

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
}
