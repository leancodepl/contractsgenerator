using LeanCode.Contracts;

public class Notification : INotification { }

public class Topic : ITopic, IProduceNotification<Notification> { }
