using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using LeanCode.Contracts;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.Serialization;

public class SubscriptionEnvelopeSerializationTests
{
    private const string SubscriptionId = "4d3b45e6-a2c1-4d6a-9e23-94e0d9f8ca01";

    private static readonly Topic SampleTopic = new() { EntityIds = ["Entity1", "Entity2"] };

    private static readonly SubscriptionEnvelope SampleSubscriptionEnvelope = new()
    {
        Id = Guid.Parse(SubscriptionId),
        TopicType = typeof(Topic).FullName!,
        Topic = JsonSerializer.SerializeToDocument(SampleTopic),
    };

    private const string Json = $$"""
        {
          "{{nameof(SubscriptionEnvelope.Id)}}": "{{SubscriptionId}}",
          "{{nameof(
            SubscriptionEnvelope.TopicType
        )}}": "LeanCode.ContractsGenerator.Tests.Serialization.SubscriptionEnvelopeSerializationTests\u002BTopic",
          "{{nameof(SubscriptionEnvelope.Topic)}}": {
            "{{nameof(Topic.EntityIds)}}": [
              "Entity1",
              "Entity2"
            ]
          }
        }
        """;

    [Fact]
    public void SubscriptionEnvelope_is_serializable()
    {
        var serialized = JsonSerializer.Serialize(
            SampleSubscriptionEnvelope,
            new JsonSerializerOptions { WriteIndented = true }
        );

        serialized.Should().Be(Json);
    }

    [Fact]
    public void SubscriptionEnvelope_is_deserializable()
    {
        using var assertionScope = new AssertionScope();

        var deserialized = JsonSerializer.Deserialize<SubscriptionEnvelope>(Json);

        deserialized.Should().BeEquivalentTo(SampleSubscriptionEnvelope, opts => opts.Excluding(e => e.Topic));

        var deserializedTopic = deserialized!.Topic.Deserialize<Topic>();

        deserializedTopic.Should().BeEquivalentTo(SampleTopic);
    }

    private sealed class Topic : ITopic
    {
        public List<string> EntityIds { get; set; } = default!;
    }
}
