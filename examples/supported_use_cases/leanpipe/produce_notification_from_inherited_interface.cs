using LeanCode.Contracts;

public class Notification { }

public interface IProducer : IProduceNotification<Notification> { }

public class Topic : ITopic, IProducer { }
