using LeanCode.Contracts;

public class Notification { }
public abstract class TopicBase : ITopic { }
public class Topic : TopicBase, IProduceNotification<Notification> { }
