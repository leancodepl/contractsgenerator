using LeanCode.CQRS;
using LeanCode.CQRS.Security;

public class Dto
{
    public decimal Wrong1 { get; set; }
    public System.Half Wrong2 { get; set; }
    public LeanCode.CQRS.ExcludeFromContractsGenerationAttribute Wrong3 { get; set; }
}

[AllowUnauthorized] public class Query : IRemoteQuery<decimal> { }
