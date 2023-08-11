using System;
using System.Collections.Generic;
using LeanCode.Contracts;

public class Topic1 : ITopic, IProduceNotification<int> { }

public class Topic2 : ITopic, IProduceNotification<DateTimeOffset> { }

public class Topic3 : ITopic, IProduceNotification<Guid> { }

public class Topic4 : ITopic, IProduceNotification<int[]> { }

public class Topic5 : ITopic, IProduceNotification<List<int>> { }

public class Topic6 : ITopic, IProduceNotification<string> { }

public class Topic7 : ITopic, IProduceNotification<Dictionary<int, string>> { }
