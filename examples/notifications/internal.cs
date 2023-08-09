using LeanCode.Contracts;

namespace Notifications.Internal;

public class Topic : ITopic, IProduceNotification<DTO1>, IProduceNotification<DTO2> { }

public class DTO1 { }

public class DTO2 { }
