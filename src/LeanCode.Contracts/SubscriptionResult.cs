namespace LeanCode.Contracts;

public class SubscriptionResult
{
    public Guid SubscriptionId { get; private init; }
    public SubscriptionStatus Status { get; private init; }
    public OperationType Type { get; private init; }

    public SubscriptionResult(Guid subscriptionId, SubscriptionStatus status, OperationType type)
    {
        SubscriptionId = subscriptionId;
        Status = status;
        Type = type;
    }
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
