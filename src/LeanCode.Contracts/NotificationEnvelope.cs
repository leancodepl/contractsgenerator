using System.Text.Json.Serialization;

namespace LeanCode.Contracts;

public sealed class NotificationEnvelope
{
    public Guid Id { get; private init; }
    public string TopicType { get; private init; }
    public string NotificationType { get; private init; }
    public object Topic { get; private init; }
    public object Notification { get; private init; }

    public NotificationEnvelope(Guid id, ITopic topic, object notification)
    {
        Id = id;
        TopicType = topic.GetType().FullName!;
        NotificationType = NotificationTagGenerator.Generate(notification.GetType());
        Topic = topic;
        Notification = notification;
    }

    public static NotificationEnvelope Create<TTopic, TNotification>(TTopic topic, TNotification notification)
        where TTopic : ITopic, IProduceNotification<TNotification>
        where TNotification : notnull
    {
        return new(Guid.NewGuid(), topic, notification);
    }

    [JsonConstructor]
    public NotificationEnvelope(Guid id, string topicType, string notificationType, object topic, object notification)
    {
        Id = id;
        TopicType = topicType;
        NotificationType = notificationType;
        Topic = topic;
        Notification = notification;
    }
}
