using System.Text;

namespace LeanCode.Contracts;

public static class NotificationTagGenerator
{
    public const string KnownTypePrefix = "!";

    /// <summary>
    /// Generates a unique tag for oncoming notifications within a specific topic based on the provided `Type`.
    /// The generated tag is used by clients to identify notifications. There is a corresponding method in
    /// `LeanCode.ContractsGenerator.Generation.NotificationTagGenerator` that generates a tag based on `TypeRef`.
    /// Both methods generate the same tags.
    /// </summary>
    public static string Generate(Type type)
    {
        return type switch
        {
            _ when TryKnownType(type) is string name
                => type.GetElementType() is Type t ?
                    $"{KnownTypePrefix}{name}{GetArgumentsString(t)}"
                    : $"{KnownTypePrefix}{name}{GetArgumentsString(type.GetGenericArguments())}",
            _ when type.IsGenericType => $"{type.GetSimpleName()}{GetArgumentsString(type.GetGenericArguments())}",
            _ => type.FullName!,
        };
    }

    private static string GetSimpleName(this Type type)
    {
        var typeName = type.FullName!;
        int backtickIndex = typeName.IndexOf('`');

        if (backtickIndex > 0)
        {
            typeName = typeName.Substring(0, backtickIndex);
        }

        return typeName;
    }

    private static string GetArgumentsString(params Type[] args)
    {
        var argsBuilder = new StringBuilder();

        if (args.Any())
        {
            argsBuilder.Append('[');

            foreach (var arg in args)
            {
                if (argsBuilder.Length > 1)
                {
                    argsBuilder.Append(',');
                }

                var argName = Generate(arg);
                argsBuilder.Append(argName);
            }

            argsBuilder.Append(']');
        }

        return argsBuilder.ToString();
    }

    private static string? TryKnownType(Type type)
    {
        return type switch
        {
            _ when type == typeof(object) => "Object",
            _ when type == typeof(string) => "String",
            _ when type == typeof(sbyte) => "Int8",
            _ when type == typeof(byte) => "Uint8",
            _ when type == typeof(short) => "Int16",
            _ when type == typeof(ushort) => "Uint16",
            _ when type == typeof(int) => "Int32",
            _ when type == typeof(uint) => "Uint32",
            _ when type == typeof(long) => "Int64",
            _ when type == typeof(ulong) => "Uint64",
            _ when type == typeof(float) => "Float32",
            _ when type == typeof(double) => "Float64",
            _ when type == typeof(bool) => "Boolean",
            _ when type == typeof(DateTimeOffset) => "DateTimeOffset",
            _ when type == typeof(DateOnly) => "DateOnly",
            _ when type == typeof(TimeOnly) => "TimeOnly",
            _ when type == typeof(Guid) => "Guid",
            _ when type == typeof(Uri) => "Uri",
            _ when type == typeof(TimeSpan) => "TimeSpan",
            _ when type.IsGenericType
                && type.GetGenericArguments().Length == 2
                && (type.GetInterfaces()
                    .Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                        || i.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)))
                    || type.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                    || type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)) => "Map",
            _ when type.IsGenericType
                && type.GetGenericArguments().Length == 1
                && (type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) => "Array",
            _ when type.IsArray => "Array",
            _ => null,
        };
    }
}
