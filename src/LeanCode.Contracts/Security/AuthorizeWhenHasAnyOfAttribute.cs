namespace LeanCode.Contracts.Security;

public interface IHasPermissions;

public sealed class AuthorizeWhenHasAnyOfAttribute(params string[] permissions)
    : AuthorizeWhenAttribute(typeof(IHasPermissions), permissions);
