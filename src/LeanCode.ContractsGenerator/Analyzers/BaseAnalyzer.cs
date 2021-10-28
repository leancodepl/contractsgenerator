namespace LeanCode.ContractsGenerator.Analyzers;

public class BaseAnalyzer : IAnalyzer
{
    public virtual IEnumerable<AnalyzeError> Analyze(Export export)
    {
        var context = AnalyzerContext.Empty;
        return export.Statements
            .SelectMany(s => AnalyzeStatement(context.Descend(s), s))
            .ToList();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeKnownType(AnalyzerContext context, KnownType knownType)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeValueRef(AnalyzerContext context, ValueRef knownType)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeGenericTypeRef(AnalyzerContext context, TypeRef typeRef, TypeRef.Types.Generic g)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeInternalTypeRef(AnalyzerContext context, TypeRef typeRef, TypeRef.Types.Internal i)
    {
        return i.Arguments.SelectMany(a => AnalyzeTypeRef(context.Marked(PathMarker.Argument).Descend(a), a));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeKnownTypeRef(AnalyzerContext context, TypeRef typeRef, TypeRef.Types.Known k)
    {
        return k.Arguments
            .SelectMany(r => AnalyzeTypeRef(context.Marked(PathMarker.Argument).Descend(r), r))
            .Concat(AnalyzeKnownType(context, k.Type));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeTypeRef(AnalyzerContext context, TypeRef typeRef)
    {
        if (typeRef.Internal is TypeRef.Types.Internal i)
        {
            return AnalyzeInternalTypeRef(context, typeRef, i);
        }
        else if (typeRef.Known is TypeRef.Types.Known k)
        {
            return AnalyzeKnownTypeRef(context, typeRef, k);
        }
        else if (typeRef.Generic is TypeRef.Types.Generic g)
        {
            return AnalyzeGenericTypeRef(context, typeRef, g);
        }
        else
        {
            return Enumerable.Empty<AnalyzeError>();
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeGenericParameter(AnalyzerContext context, GenericParameter genericParam)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzePositionalAttributeArgument(AnalyzerContext context, AttributeArgument arg, AttributeArgument.Types.Positional p)
    {
        return AnalyzeValueRef(context, p.Value);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeNamedAttributeArgument(AnalyzerContext context, AttributeArgument arg, AttributeArgument.Types.Named n)
    {
        return AnalyzeValueRef(context, n.Value);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeAttributeArgument(AnalyzerContext context, AttributeArgument arg)
    {
        if (arg.Positional is AttributeArgument.Types.Positional p)
        {
            return AnalyzePositionalAttributeArgument(context, arg, p);
        }
        else if (arg.Named is AttributeArgument.Types.Named n)
        {
            return AnalyzeNamedAttributeArgument(context, arg, n);
        }
        else
        {
            return Enumerable.Empty<AnalyzeError>();
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeAttributeRef(AnalyzerContext context, AttributeRef attrRef)
    {
        return attrRef.Argument.SelectMany(a => AnalyzeAttributeArgument(context.Marked(PathMarker.Argument).Descend(a), a));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzePropertyRef(AnalyzerContext context, PropertyRef propRef)
    {
        return AnalyzeTypeRef(context, propRef.Type)
            .Concat(propRef.Attributes.SelectMany(a => AnalyzeAttributeRef(context.Marked(PathMarker.Attribute).Descend(a), a)));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeConstantRef(AnalyzerContext context, ConstantRef constant)
    {
        return AnalyzeValueRef(context, constant.Value);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeEnumValue(AnalyzerContext context, EnumValue enumVal)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeGroupErrorCode(AnalyzerContext context, ErrorCode errCode, ErrorCode.Types.Group g)
    {
        return AnalyzeErrorCodes(context, g.InnerCodes);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeSingleErrorCode(AnalyzerContext context, ErrorCode errCode, ErrorCode.Types.Single s)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeErrorCode(AnalyzerContext context, ErrorCode errCode)
    {
        if (errCode.Group is ErrorCode.Types.Group g)
        {
            return AnalyzeGroupErrorCode(context, errCode, g);
        }
        else if (errCode.Single is ErrorCode.Types.Single s)
        {
            return AnalyzeSingleErrorCode(context, errCode, s);
        }
        else
        {
            return Enumerable.Empty<AnalyzeError>();
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeErrorCodes(AnalyzerContext context, IEnumerable<ErrorCode> errCodes)
    {
        return errCodes.SelectMany(e => AnalyzeErrorCode(context.Descend(e), e));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeTypeDescriptor(AnalyzerContext context, TypeDescriptor descr)
    {
        return descr.Extends.SelectMany(t => AnalyzeTypeRef(context.Marked(PathMarker.Extends).Descend(t), t))
            .Concat(descr.GenericParameters.SelectMany(g => AnalyzeGenericParameter(context.Marked(PathMarker.GenericParameter).Descend(g), g)))
            .Concat(descr.Properties.SelectMany(p => AnalyzePropertyRef(context.Descend(p), p)))
            .Concat(descr.Constants.SelectMany(c => AnalyzeConstantRef(context.Descend(c), c)));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeDTO(AnalyzerContext context, Statement stmt, Statement.Types.DTO dto)
    {
        return AnalyzeTypeDescriptor(context, dto.TypeDescriptor);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeEnum(AnalyzerContext context, Statement stmt, Statement.Types.Enum @enum)
    {
        return @enum.Members.SelectMany(m => AnalyzeEnumValue(context.Descend(m), m));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeQuery(AnalyzerContext context, Statement stmt, Statement.Types.Query query)
    {
        return AnalyzeTypeDescriptor(context, query.TypeDescriptor)
            .Concat(AnalyzeTypeRef(context.Marked(PathMarker.ReturnType), query.ReturnType));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeCommand(AnalyzerContext context, Statement stmt, Statement.Types.Command command)
    {
        return AnalyzeTypeDescriptor(context, command.TypeDescriptor)
            .Concat(AnalyzeErrorCodes(context.Marked(PathMarker.ErrorCodes), command.ErrorCodes));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeStatement(AnalyzerContext context, Statement stmt)
    {
        return AnalyzeInner(context, stmt)
            .Concat(stmt.Attributes.SelectMany(a => AnalyzeAttributeRef(context.Marked(PathMarker.Attribute).Descend(a), a)));

        IEnumerable<AnalyzeError> AnalyzeInner(AnalyzerContext context, Statement stmt)
        {
            if (stmt.Dto is Statement.Types.DTO dto)
            {
                return AnalyzeDTO(context, stmt, dto);
            }
            else if (stmt.Enum is Statement.Types.Enum @enum)
            {
                return AnalyzeEnum(context, stmt, @enum);
            }
            else if (stmt.Query is Statement.Types.Query query)
            {
                return AnalyzeQuery(context, stmt, query);
            }
            else if (stmt.Command is Statement.Types.Command cmd)
            {
                return AnalyzeCommand(context, stmt, cmd);
            }
            else
            {
                return Enumerable.Empty<AnalyzeError>();
            }
        }
    }
}
