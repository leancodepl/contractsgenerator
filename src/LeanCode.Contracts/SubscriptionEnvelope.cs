using System.Text.Json;

namespace LeanCode.Contracts;

public sealed class SubscriptionEnvelope
{
    public Guid Id { get; set; }
    public string TopicType { get; set; } = default!;
    public JsonDocument Topic { get; set; } = default!;
}
