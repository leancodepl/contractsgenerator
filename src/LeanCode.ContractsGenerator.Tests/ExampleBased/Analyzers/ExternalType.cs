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
                .WithError("CNTR0004", "Dto.Wrong1")
                .WithError("CNTR0004", "Dto.Wrong2")
                .WithError("CNTR0004", "Dto.Wrong3")
                .WithError("CNTR0004", "Query->System.Decimal");
    }
}
