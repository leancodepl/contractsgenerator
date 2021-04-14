using LeanCode.CQRS;
using LeanCode.CQRS.Security;

namespace LeanCode.ContractsGeneratorV2.ExampleContracts.Users
{
    [AuthorizeWhenHasAnyOf(Auth.Roles.Admin)]
    public class UserSomething : IRemoteQuery<int?>
    { }
}