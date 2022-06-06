using System.Reflection;

namespace LeanCode.Contracts.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
public abstract class AuthorizeWhenAttribute : Attribute
{
    private readonly Type authorizerType;
    private readonly object? customData;

    protected AuthorizeWhenAttribute(Type authorizerType, object? customData = null)
    {
        this.authorizerType = authorizerType;
        this.customData = customData;
    }

    public static List<AuthorizerDefinition> GetCustomAuthorizers(Type type)
    {
        return type
            .GetCustomAttributes<AuthorizeWhenAttribute>()
            .Select(AuthorizerDefinition.Create)
            .ToList();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("?", "CA1034", Justification = "Deliberate nesting.")]
    public sealed class AuthorizerDefinition
    {
        public Type Authorizer { get; }
        public object? CustomData { get; }

        private AuthorizerDefinition(AuthorizeWhenAttribute attr)
        {
            Authorizer = attr.authorizerType;
            CustomData = attr.customData;
        }

        internal static AuthorizerDefinition Create(AuthorizeWhenAttribute attr) =>
            new(attr);
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
public abstract class AuthorizeWhenAttribute<T> : AuthorizeWhenAttribute
{
    protected AuthorizeWhenAttribute(object? customData = null)
        : base(typeof(T), customData)
    { }

    public static List<AuthorizerDefinition> GetCustomAuthorizers() =>
        GetCustomAuthorizers(typeof(T));
}
