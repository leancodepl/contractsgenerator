using AT = System.AttributeTargets;

namespace LeanCode.Contracts;

[AttributeUsage(
    AT.Class | AT.Struct | AT.Enum | AT.Interface | AT.Property | AT.Field,
    AllowMultiple = false,
    Inherited = false
)]
public sealed class ExcludeFromContractsGenerationAttribute : Attribute { }
