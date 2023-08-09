using LeanCode.Contracts;

public class Notification { }

public class TopicBase : ITopic, IProduceNotification<Notification> { }

public class Topic : TopicBase { }
