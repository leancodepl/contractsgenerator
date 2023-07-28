using System;
using System.Collections.Generic;
using LeanCode.Contracts;

namespace Notifications;

public class Topic1 : ITopic, IProduceNotification<int> { }

public class Topic2 : ITopic, IProduceNotification<bool> { }

public class Topic3 : ITopic, IProduceNotification<DateTimeOffset> { }

public class Topic4 : ITopic, IProduceNotification<Guid> { }

public class Topic5 : ITopic, IProduceNotification<int[]> { }

public class Topic6 : ITopic, IProduceNotification<int[,,][]> { }

public class Topic7 : ITopic, IProduceNotification<object> { }

public class Topic8 : ITopic, IProduceNotification<List<int>> { }

public class Topic9 : ITopic, IProduceNotification<string> { }

public class Topic10 : ITopic, IProduceNotification<Dictionary<int, string>> { }
