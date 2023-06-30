namespace LeanCode.Contracts;

public sealed class NotificationEnvelope
{
    public Guid Id { get; private init; }
    public string TopicType { get; private init; } = default!;
    public string NotificationType { get; private init; } = default!;
    public object Topic { get; private init; } = default!;
    public object Notification { get; private init; } = default!;

    public static NotificationEnvelope Create<TTopic, TNotification>(TTopic topic, TNotification notification)
        where TTopic : ITopic, IProduceNotification<TNotification>
        where TNotification : notnull
    {
        return new()
        {
            Id = Guid.NewGuid(),
            TopicType = typeof(TTopic).FullName!,
            NotificationType = typeof(TNotification).FullName!,
            Topic = topic,
            Notification = notification,
        };
    }
}
