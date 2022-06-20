using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace LeanCode.ContractsGenerator;

[SuppressMessage("?", "SA1313", Justification = "False positive.")]
public readonly record struct AnalyzerContext(string Path)
{
    public static readonly AnalyzerContext Empty = new("");

    public AnalyzerContext Descend(Statement stmt) => Descend(stmt.Name);
    public AnalyzerContext Descend(EnumValue e) => Descend(e.Name);
    public AnalyzerContext Descend(PropertyRef propRef) => Descend(propRef.Name);
    public AnalyzerContext Descend(ConstantRef constRef) => Descend(constRef.Name);

    public AnalyzerContext Argument(int pos, TypeRef t) => Append($"<{pos}: {NameOf(t)}>");
    public AnalyzerContext Argument(AttributeArgument t) => Append($"({NameOf(t)})");
    public AnalyzerContext Attribute(AttributeRef attr) => Append($"[{attr.AttributeName}]");
    public AnalyzerContext GenericParameter(int pos, GenericParameter p) => Append($"<{pos}: {p.Name}>");
    public AnalyzerContext Extends(TypeRef t) => Append($":{NameOf(t)}");
    public AnalyzerContext Returns(TypeRef t) => Append($"->{NameOf(t)}");
    public AnalyzerContext ErrorCodes() => Descend("ErrorCodes");

    public AnalyzerContext Descend(ErrorCode errCode)
    {
        if (errCode.Group is ErrorCode.Types.Group g)
        {
            return Descend(g.Name);
        }
        else if (errCode.Single is ErrorCode.Types.Single s)
        {
            return Descend(s.Name);
        }
        else
        {
            return this;
        }
    }

    private AnalyzerContext Descend(string nextName)
    {
        if (this == Empty)
        {
            return new(nextName);
        }
        else
        {
            return new($"{Path}.{nextName}");
        }
    }

    private AnalyzerContext Append(string nextName)
    {
        return new($"{Path}{nextName}");
    }

    private static string NameOf(TypeRef typeRef)
    {
        if (typeRef.Internal is TypeRef.Types.Internal i)
        {
            return i.Name;
        }
        else if (typeRef.Known is TypeRef.Types.Known k)
        {
            return k.Type.ToString();
        }
        else if (typeRef.Generic is TypeRef.Types.Generic g)
        {
            return g.Name;
        }
        else
        {
            return "";
        }
    }

    private static string NameOf(AttributeArgument arg)
    {
        if (arg.Positional is AttributeArgument.Types.Positional p)
        {
            return p.Position.ToString(CultureInfo.InvariantCulture);
        }
        else if (arg.Named is AttributeArgument.Types.Named n)
        {
            return n.Name;
        }
        else
        {
            return "";
        }
    }
}
