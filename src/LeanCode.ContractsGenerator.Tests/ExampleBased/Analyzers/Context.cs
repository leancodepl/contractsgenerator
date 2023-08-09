using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers;

public class Context
{
    [Fact]
    public void Context_is_tracked_correctly()
    {
        "analyzers/context.cs"
            .AnalyzeFails()
            .WithError("CNTR0004", "Dto1.A<0: System.Decimal>")
            .WithError("CNTR0004", "Dto2.A<0: System.Decimal>")
            .WithError("CNTR0004", "Dto3:System.IDisposable")
            .WithError("CNTR0004", "Dto4:System.IDisposable")
            .WithError("CNTR0004", "Dto5:Inner<0: System.Decimal>")
            .WithError("CNTR0004", "Dto6:Inner<0: Inner><0: System.Decimal>")
            .WithError("CNTR0004", "Query1->System.Decimal")
            .WithError("CNTR0004", "Query2->Inner<0: System.Decimal>");
    }
}
