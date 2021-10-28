using System.Collections.Generic;
using LeanCode.CQRS;
using LeanCode.CQRS.Security;

public class Inner<T> { }

public class Dto1 { public Inner<decimal> A { get; set; } }
public class Dto2 { public List<decimal> A { get; set; } }
public class Dto3 : System.IDisposable { public void Dispose() { } }
[AllowUnauthorized] public class Query1 : IRemoteQuery<decimal> { }
[AllowUnauthorized] public class Query2 : IRemoteQuery<Inner<decimal>> { }
