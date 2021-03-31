using LeanCode.CQRS.Security;

namespace LeanCode.ContractsGeneratorV2.ExampleContracts.Users
{
    [AuthorizeWhenHasAnyOf(Auth.Roles.Admin)]
    public class AllUsers : PaginatedQuery<UserInfoDTO>
    { }
}