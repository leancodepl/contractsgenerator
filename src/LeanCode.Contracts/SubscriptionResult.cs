namespace LeanCode.Contracts;

public sealed class SubscriptionResult(Guid subscriptionId, SubscriptionStatus status, OperationType type)
{
    public Guid SubscriptionId { get; private init; } = subscriptionId;
    public SubscriptionStatus Status { get; private init; } = status;
    public OperationType Type { get; private init; } = type;
}

public enum SubscriptionStatus
{
    Success = 0,
    Unauthorized = 1,
    Malformed = 2,
    Invalid = 3,
    InternalServerError = 4,
}

public enum OperationType
{
    Subscribe = 0,
    Unsubscribe = 1,
}
