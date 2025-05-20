using System.Text.Json;

namespace LeanCode.Contracts;

public sealed class SubscriptionEnvelope : IDisposable
{
    public Guid Id { get; set; }
    public string TopicType { get; set; } = default!;
    public JsonDocument Topic { get; set; } = default!;

    public void Dispose() => Topic.Dispose();
}
