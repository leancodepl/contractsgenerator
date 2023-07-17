using LeanCode.Contracts;

public class Notification { }
public class Producer : IProduceNotification<Notification> { }
public class Topic : Producer, ITopic { }
