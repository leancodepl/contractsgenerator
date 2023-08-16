using LeanCode.ContractsGenerator.Analyzers;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers;

public class TopicWithoutNotification
{
    [Fact]
    public void Topics_without_notifications_are_reported()
    {
        "analyzers/topic_without_notification.cs"
            .AnalyzeFails()
            .WithErrorNumber(4)
            .WithError(
                AnalyzerCodes.TopicDoesNotProduceNotification,
                "EmptyTopic",
                messagePattern: "Topic type .+ doesn't produce any notification."
            )
            .WithError(
                AnalyzerCodes.TopicDoesNotProduceNotification,
                "EmptyInheritedTopic",
                messagePattern: "Topic type .+ doesn't produce any notification."
            )
            .WithError(
                AnalyzerCodes.TopicDoesNotProduceNotification,
                "InheritedInterfaceEmptyTopic",
                messagePattern: "Topic type .+ doesn't produce any notification."
            )
            .WithError(
                AnalyzerCodes.TopicDoesNotProduceNotification,
                "ConcreteEmptyTopic",
                messagePattern: "Topic type .+ doesn't produce any notification."
            );
    }
}
