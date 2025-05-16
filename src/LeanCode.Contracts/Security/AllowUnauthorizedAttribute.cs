namespace LeanCode.Contracts.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public sealed class AllowUnauthorizedAttribute : Attribute;
