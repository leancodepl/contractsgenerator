using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using LeanCode.Contracts;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.Serialization;

public class NotificationEnvelopeSerializationTests
{
    private const string NotificationId = "4d3b45e6-a2c1-4d6a-9e23-94e0d9f8ca01";

    private static readonly Topic SampleTopic = new() { EntityIds = ["Entity1", "Entity2"] };

    private static readonly Notification SampleNotification = new() { EntityId = "Entity1" };

    private static readonly NotificationEnvelope SampleNotificationEnvelope = new(
        Guid.Parse(NotificationId),
        SampleTopic,
        SampleNotification
    );

    private const string Json = $$"""
        {
          "{{nameof(NotificationEnvelope.Id)}}": "{{NotificationId}}",
          "{{nameof(
            NotificationEnvelope.TopicType
        )}}": "LeanCode.ContractsGenerator.Tests.Serialization.NotificationEnvelopeSerializationTests\u002BTopic",
          "{{nameof(
            NotificationEnvelope.NotificationType
        )}}": "LeanCode.ContractsGenerator.Tests.Serialization.NotificationEnvelopeSerializationTests\u002BNotification",
          "{{nameof(NotificationEnvelope.Topic)}}": {
            "{{nameof(Topic.EntityIds)}}": [
              "Entity1",
              "Entity2"
            ]
          },
          "{{nameof(NotificationEnvelope.Notification)}}": {
            "{{nameof(Notification.EntityId)}}": "Entity1"
          }
        }
        """;

    [Fact]
    public void NotificationEnvelope_is_serializable()
    {
        var serialized = JsonSerializer.Serialize(
            SampleNotificationEnvelope,
            new JsonSerializerOptions { WriteIndented = true }
        );

        serialized.Should().Be(Json);
    }

    [Fact]
    public void NotificationEnvelope_is_deserializable()
    {
        using var assertionScope = new AssertionScope();

        var deserialized = JsonSerializer.Deserialize<NotificationEnvelope>(Json);

        deserialized
            .Should()
            .BeEquivalentTo(
                SampleNotificationEnvelope,
                opts => opts.Excluding(e => e.Topic).Excluding(e => e.Notification)
            );

        var deserializedTopic = ((JsonElement)deserialized!.Topic).Deserialize<Topic>();

        deserializedTopic.Should().BeEquivalentTo(SampleTopic);

        var deserializedNotification = ((JsonElement)deserialized!.Notification).Deserialize<Notification>();

        deserializedNotification.Should().BeEquivalentTo(SampleNotification);
    }

    private class Topic : ITopic, IProduceNotification<Notification>
    {
        public List<string> EntityIds { get; set; } = default!;
    }

    private class Notification
    {
        public string EntityId { get; set; } = default!;
    }
}
