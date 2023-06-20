using LeanCode.Contracts;

public class Notification { }

public class Topic : ITopic, IProduceNotification<Notification> { }
