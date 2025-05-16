namespace LeanCode.ContractsGenerator.Tests;

public static class NotificationTypeRefExtensions
{
    public static NotificationTypeRef WithTag(TypeRef typeRef, string tag)
    {
        return new() { Type = typeRef, Tag = tag };
    }
}
