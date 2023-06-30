namespace LeanCode.Contracts;

public class SubscriptionEnvelope
{
    public Guid Id { get; set; }
    public string TopicType { get; set; } = default!;
    public string Topic { get; set; } = default!;
}
