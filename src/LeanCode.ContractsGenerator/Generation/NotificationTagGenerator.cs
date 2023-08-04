using System.Text;

namespace LeanCode.ContractsGenerator.Generation;

public static class NotificationTagGenerator
{
    /// <summary>
    /// Generates a unique tag for oncoming notifications within a specific topic based on the provided `TypeRef`.
    /// The generated tag is used by clients to identify notifications. There is a corresponding method in
    /// `LeanCode.Contracts.NotificationTagGenerator` that generates a tag based on `Type`.
    /// Both methods generate the same tags.
    /// </summary>
    public static string Generate(TypeRef typeRef)
    {
        return typeRef switch
        {
            { Internal: TypeRef.Types.Internal i } => $"{i.Name}{GetArgumentsString(i.Arguments)}",
            { Generic: TypeRef.Types.Generic g } => g.Name,
            { Known: TypeRef.Types.Known k }
                => $"{LeanCode.Contracts.NotificationTagGenerator.KnownTypePrefix}{k.Type}{GetArgumentsString(k.Arguments)}",
            _ => throw new InvalidOperationException($"Unknown TypeRef: {typeRef}."),
        };
    }

    private static string GetArgumentsString(IEnumerable<TypeRef> args)
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
}
