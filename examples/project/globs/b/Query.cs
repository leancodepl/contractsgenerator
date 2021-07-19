using LeanCode.CQRS;
using LeanCode.CQRS.Security;

namespace B
{
    [AllowUnauthorized]
    public class Query : IRemoteQuery<int>
    { }
}
