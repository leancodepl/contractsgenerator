using LeanCode.Contracts;

namespace C
{
    public class Notification { }

    public class Topic : ITopic, IProduceNotification<Notification> { }
}
