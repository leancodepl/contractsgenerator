using LeanCode.ContractsGenerator.Analyzers;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers;

public class ExternalType
{
    [Fact]
    public void External_types_are_reported_as_errors()
    {
        "analyzers/external_types.cs"
            .AnalyzeFails()
            .WithErrorNumber(4)
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto.Wrong1")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto.Wrong2")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto.Wrong3")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Query->System.Decimal");
    }
}
