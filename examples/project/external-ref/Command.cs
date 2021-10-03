using LeanCode.CQRS;
using LeanCode.CQRS.Security;

namespace ExternalRef
{
    [AllowUnauthorized]
    public class Command : IRemoteCommand
    { }
}
