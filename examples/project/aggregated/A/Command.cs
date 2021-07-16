using LeanCode.CQRS;
using LeanCode.CQRS.Security;

namespace A
{
    [AllowUnauthorized]
    public class Command : IRemoteCommand
    { }
}
