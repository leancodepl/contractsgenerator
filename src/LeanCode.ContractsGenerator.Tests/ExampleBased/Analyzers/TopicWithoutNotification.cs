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
                .WithError("CNTR0007", "EmptyTopic", messagePattern: "Topic type .+ doesn't produce any notification.")
                .WithError("CNTR0007", "EmptyInheritedTopic", messagePattern: "Topic type .+ doesn't produce any notification.")
                .WithError("CNTR0007", "InheritedInterfaceEmptyTopic", messagePattern: "Topic type .+ doesn't produce any notification.")
                .WithError("CNTR0007", "ConcreteEmptyTopic", messagePattern: "Topic type .+ doesn't produce any notification.");
    }
}
