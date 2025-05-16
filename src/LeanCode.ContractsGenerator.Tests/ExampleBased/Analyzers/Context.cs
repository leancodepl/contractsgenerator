using LeanCode.ContractsGenerator.Analyzers;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers;

public class Context
{
    [Fact]
    public void Context_is_tracked_correctly()
    {
        "analyzers/context.cs"
            .AnalyzeFails()
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto1.A<0: System.Decimal>")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto2.A<0: System.Decimal>")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto3:System.IDisposable")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto4:System.IDisposable")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto5:Inner<0: System.Decimal>")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Dto6:Inner<0: Inner><0: System.Decimal>")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Query1->System.Decimal")
            .WithError(AnalyzerCodes.InternalTypeIsNotKnown, "Query2->Inner<0: System.Decimal>");
    }
}
