using System;
using System.Collections.Generic;
using LeanCode.Contracts;

namespace Notifications;

public class Topic1 : ITopic, IProduceNotification<Notification1<int>> { }

public class Topic2 : ITopic, IProduceNotification<Notification2<int, DTO1>> { }

public class Topic3 : ITopic, IProduceNotification<Notification2<DateTimeOffset, DTO2<int>>> { }

public class Topic4 : ITopic, IProduceNotification<Dictionary<int, DTO2<int>>> { }


public class Notification1<T> { }

public class Notification2<T1, T2> { }

public class DTO1 { }

public class DTO2<T> { }
