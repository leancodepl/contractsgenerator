namespace LeanCode.ContractsGenerator.Analyzers;

public class BaseAnalyzer : IAnalyzer
{
    public virtual IEnumerable<AnalyzeError> Analyze(Export export)
    {
        return export.Statements
            .SelectMany(AnalyzeStatement)
            .ToList();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeKnownType(KnownType knownType)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeValueRef(ValueRef knownType)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeGenericTypeRef(TypeRef typeRef, TypeRef.Types.Generic g)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeInternalTypeRef(TypeRef typeRef, TypeRef.Types.Internal i)
    {
        return i.Arguments.SelectMany(AnalyzeTypeRef);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeKnownTypeRef(TypeRef typeRef, TypeRef.Types.Known k)
    {
        return k.Arguments
            .SelectMany(AnalyzeTypeRef)
            .Concat(AnalyzeKnownType(k.Type));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeTypeRef(TypeRef typeRef)
    {
        if (typeRef.Internal is TypeRef.Types.Internal i)
        {
            return AnalyzeInternalTypeRef(typeRef, i);
        }
        else if (typeRef.Known is TypeRef.Types.Known k)
        {
            return AnalyzeKnownTypeRef(typeRef, k);
        }
        else if (typeRef.Generic is TypeRef.Types.Generic g)
        {
            return AnalyzeGenericTypeRef(typeRef, g);
        }
        else
        {
            return Enumerable.Empty<AnalyzeError>();
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeGenericParameter(GenericParameter genericParam)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzePositionalAttributeArgument(AttributeArgument arg, AttributeArgument.Types.Positional p)
    {
        return AnalyzeValueRef(p.Value);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeNamedAttributeArgument(AttributeArgument arg, AttributeArgument.Types.Named n)
    {
        return AnalyzeValueRef(n.Value);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeAttributeArgument(AttributeArgument arg)
    {
        if (arg.Positional is AttributeArgument.Types.Positional p)
        {
            return AnalyzePositionalAttributeArgument(arg, p);
        }
        else if (arg.Named is AttributeArgument.Types.Named n)
        {
            return AnalyzeNamedAttributeArgument(arg, n);
        }
        else
        {
            return Enumerable.Empty<AnalyzeError>();
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeAttributeRef(AttributeRef attrRef)
    {
        return attrRef.Argument.SelectMany(AnalyzeAttributeArgument);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzePropertyRef(PropertyRef propRef)
    {
        return AnalyzeTypeRef(propRef.Type)
            .Concat(propRef.Attributes.SelectMany(AnalyzeAttributeRef));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeConstantRef(ConstantRef constant)
    {
        return AnalyzeValueRef(constant.Value);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeEnumValue(EnumValue enumVal)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeGroupErrorCode(ErrorCode errCode, ErrorCode.Types.Group g)
    {
        return AnalyzeErrorCodes(g.InnerCodes);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeSingleErrorCode(ErrorCode errCode, ErrorCode.Types.Single s)
    {
        return Enumerable.Empty<AnalyzeError>();
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeErrorCode(ErrorCode errCode)
    {
        if (errCode.Group is ErrorCode.Types.Group g)
        {
            return AnalyzeGroupErrorCode(errCode, g);
        }
        else if (errCode.Single is ErrorCode.Types.Single s)
        {
            return AnalyzeSingleErrorCode(errCode, s);
        }
        else
        {
            return Enumerable.Empty<AnalyzeError>();
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeErrorCodes(IEnumerable<ErrorCode> errCodes)
    {
        return errCodes.SelectMany(AnalyzeErrorCode);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeTypeDescriptor(TypeDescriptor descr)
    {
        return descr.Extends.SelectMany(AnalyzeTypeRef)
            .Concat(descr.GenericParameters.SelectMany(AnalyzeGenericParameter))
            .Concat(descr.Properties.SelectMany(AnalyzePropertyRef))
            .Concat(descr.Constants.SelectMany(AnalyzeConstantRef));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeDTO(Statement stmt, Statement.Types.DTO dto)
    {
        return AnalyzeTypeDescriptor(dto.TypeDescriptor);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeEnum(Statement stmt, Statement.Types.Enum @enum)
    {
        return @enum.Members.SelectMany(AnalyzeEnumValue);
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeQuery(Statement stmt, Statement.Types.Query query)
    {
        return AnalyzeTypeDescriptor(query.TypeDescriptor)
            .Concat(AnalyzeTypeRef(query.ReturnType));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeCommand(Statement stmt, Statement.Types.Command command)
    {
        return AnalyzeTypeDescriptor(command.TypeDescriptor)
            .Concat(AnalyzeErrorCodes(command.ErrorCodes));
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeStatement(Statement stmt)
    {
        return AnalyzeInner(stmt)
            .Concat(stmt.Attributes.SelectMany(AnalyzeAttributeRef));

        IEnumerable<AnalyzeError> AnalyzeInner(Statement stmt)
        {
            if (stmt.Dto is Statement.Types.DTO dto)
            {
                return AnalyzeDTO(stmt, dto);
            }
            else if (stmt.Enum is Statement.Types.Enum @enum)
            {
                return AnalyzeEnum(stmt, @enum);
            }
            else if (stmt.Query is Statement.Types.Query query)
            {
                return AnalyzeQuery(stmt, query);
            }
            else if (stmt.Command is Statement.Types.Command cmd)
            {
                return AnalyzeCommand(stmt, cmd);
            }
            else
            {
                return Enumerable.Empty<AnalyzeError>();
            }
        }
    }
}
