using System.Diagnostics.CodeAnalysis;

namespace LeanCode.ContractsGenerator;

public enum PathMarker
{
    Argument,
    Attribute,
    GenericParameter,
    Extends,
    ReturnType,
    ErrorCodes,
}

[SuppressMessage("?", "SA1313", Justification = "False positive.")]
public readonly record struct AnalyzerContext(string Path)
{
    public static readonly AnalyzerContext Empty = new("");

    public AnalyzerContext Descend(AttributeRef attrRef)
    {
        return Descend(attrRef.AttributeName);
    }

    public AnalyzerContext Descend(Statement stmt)
    {
        return Descend(stmt.Name);
    }

    public AnalyzerContext Descend(EnumValue e)
    {
        return Descend(e.Name);
    }

    public AnalyzerContext Descend(GenericParameter g)
    {
        return Descend(g.Name);
    }

    public AnalyzerContext Descend(PropertyRef propRef)
    {
        return Descend(propRef.Name);
    }

    public AnalyzerContext Descend(ConstantRef constRef)
    {
        return Descend(constRef.Name);
    }

    public AnalyzerContext Marked(PathMarker marker)
    {
        return marker switch
        {
            PathMarker.Argument => Descend("`Arg`"),
            PathMarker.Attribute => Descend("`Attr`"),
            PathMarker.GenericParameter => Descend("`Generic`"),
            PathMarker.Extends => Descend("`Extends`"),
            PathMarker.ReturnType => Descend("`Return`"),
            PathMarker.ErrorCodes => Descend("ErrorCodes"),
            _ => this,
        };
    }

    public AnalyzerContext Descend(TypeRef typeRef)
    {
        if (typeRef.Internal is TypeRef.Types.Internal i)
        {
            return Descend(i.Name);
        }
        else if (typeRef.Known is TypeRef.Types.Known k)
        {
            return Descend(k.Type.ToString());
        }
        else if (typeRef.Generic is TypeRef.Types.Generic g)
        {
            return Descend(g.Name);
        }
        else
        {
            return this;
        }
    }

    public AnalyzerContext Descend(AttributeArgument arg)
    {
        if (arg.Positional is AttributeArgument.Types.Positional p)
        {
            return Descend(p.Position.ToString());
        }
        else if (arg.Named is AttributeArgument.Types.Named n)
        {
            return Descend(n.Name);
        }
        else
        {
            return this;
        }
    }

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
}
