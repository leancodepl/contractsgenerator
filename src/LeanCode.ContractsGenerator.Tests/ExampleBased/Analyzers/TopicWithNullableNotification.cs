using LeanCode.ContractsGenerator.Analyzers;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers;

public class TopicWithNullableNotification
{
    [Fact]
    public void Topics_with_nullable_notifications_are_reported()
    {
        "analyzers/topic_with_nullable_notifications.cs"
            .AnalyzeFails()
            .WithErrorNumber(2)
            .WithError(
                AnalyzerCodes.TopicProducesNullableNotification,
                "NullableNotificationTopic",
                messagePattern: "Topic type .+ produces nullable notification type"
            )
            .WithError(
                AnalyzerCodes.TopicProducesNullableNotification,
                "NullableNotificationTopic",
                messagePattern: "Topic type .+ produces nullable notification type."
            );
    }
}
