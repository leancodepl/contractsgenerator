using LeanCode.Contracts;

public class Notification1 { }
public class Notification2 { }

public class NullableNotificationTopic : ITopic, IProduceNotification<Notification1?>, IProduceNotification<Notification2?> { }
