using LeanCode.Contracts;

public class Notification
{
    public int Num { get; set; }
}

public class Topic : ITopic, IProduceNotification<Notification>
{
    public string Key { get; set; }
}
