using LeanCode.CQRS;
using LeanCode.CQRS.Security;

namespace Single
{
    [AllowUnauthorized]
    public class Command : IRemoteCommand
    { }
}
