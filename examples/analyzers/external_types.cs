using LeanCode.Contracts;
using LeanCode.Contracts.Security;

public class Dto
{
    public decimal Wrong1 { get; set; }
    public System.Half Wrong2 { get; set; }
    public LeanCode.Contracts.ExcludeFromContractsGenerationAttribute Wrong3 { get; set; }
}

[AllowUnauthorized]
public class Query : IQuery<decimal> { }
