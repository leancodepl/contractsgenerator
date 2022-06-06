using LeanCode.Contracts;
using LeanCode.Contracts.Security;

namespace B
{
    [AllowUnauthorized]
    public class Query : IQuery<int>
    { }
}
