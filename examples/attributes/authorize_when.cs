using LeanCode.Contracts;
using LeanCode.Contracts.Security;

[AuthorizeWhenCustomCtor]
public class A : ICommand { }

[AuthorizeWhenCustomGeneric]
public class B : ICommand { }

public class AuthorizeWhenCustomCtorAttribute : AuthorizeWhenAttribute
{
    public AuthorizeWhenCustomCtorAttribute()
        : base(typeof(A))
    { }
}

public class AuthorizeWhenCustomGenericAttribute : AuthorizeWhenAttribute<B> { }
