using LeanCode.Contracts;

public class FirstNotification { }
public class SecondNotification { }

public class Topic : ITopic, IProduceNotification<FirstNotification>, IProduceNotification<SecondNotification> { }
