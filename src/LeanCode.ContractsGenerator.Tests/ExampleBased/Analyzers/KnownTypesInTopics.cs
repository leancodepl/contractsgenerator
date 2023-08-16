using LeanCode.ContractsGenerator.Analyzers;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers;

public class KnownTypesInTopics
{
    [Fact]
    public void Known_types_are_forbidden_in_topics()
    {
        "analyzers/topic_known_types.cs"
            .AnalyzeFails()
            .WithErrorNumber(7)
            .WithError(AnalyzerCodes.TopicMustProduceInternalType, "Topic1")
            .WithError(AnalyzerCodes.TopicMustProduceInternalType, "Topic2")
            .WithError(AnalyzerCodes.TopicMustProduceInternalType, "Topic3")
            .WithError(AnalyzerCodes.TopicMustProduceInternalType, "Topic4")
            .WithError(AnalyzerCodes.TopicMustProduceInternalType, "Topic5")
            .WithError(AnalyzerCodes.TopicMustProduceInternalType, "Topic6")
            .WithError(AnalyzerCodes.TopicMustProduceInternalType, "Topic7");
    }
}
