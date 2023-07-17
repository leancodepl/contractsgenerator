using LeanCode.Contracts;

public class Notification { }
public interface ITopicBase : ITopic, IProduceNotification<Notification> { }
public class Topic : ITopicBase { }
