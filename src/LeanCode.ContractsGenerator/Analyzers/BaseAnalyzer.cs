namespace LeanCode.ContractsGenerator.Analyzers;

public class BaseAnalyzer : IAnalyzer
{
    public virtual IEnumerable<AnalyzeError> Analyze(Export export)
    {
        var context = AnalyzerContext.Empty;
        return [.. export.Statements.SelectMany(s => AnalyzeStatement(context.Descend(s), s))];
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeKnownType(AnalyzerContext context, KnownType knownType) => [];

    public virtual IEnumerable<AnalyzeError> AnalyzeValueRef(AnalyzerContext context, ValueRef valueRef) => [];

    public virtual IEnumerable<AnalyzeError> AnalyzeGenericTypeRef(
        AnalyzerContext context,
        TypeRef typeRef,
        TypeRef.Types.Generic g
    ) => [];

    public virtual IEnumerable<AnalyzeError> AnalyzeInternalTypeRef(
        AnalyzerContext context,
        TypeRef typeRef,
        TypeRef.Types.Internal i
    ) => i.Arguments.SelectMany((a, i) => AnalyzeTypeRef(context.Argument(i, a), a));

    public virtual IEnumerable<AnalyzeError> AnalyzeKnownTypeRef(
        AnalyzerContext context,
        TypeRef typeRef,
        TypeRef.Types.Known k
    ) =>
        k
            .Arguments.SelectMany((a, i) => AnalyzeTypeRef(context.Argument(i, a), a))
            .Concat(AnalyzeKnownType(context, k.Type));

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
            return [];
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeGenericParameter(
        AnalyzerContext context,
        GenericParameter genericParam
    ) => [];

    public virtual IEnumerable<AnalyzeError> AnalyzePositionalAttributeArgument(
        AnalyzerContext context,
        AttributeArgument arg,
        AttributeArgument.Types.Positional p
    ) => AnalyzeValueRef(context, p.Value);

    public virtual IEnumerable<AnalyzeError> AnalyzeNamedAttributeArgument(
        AnalyzerContext context,
        AttributeArgument arg,
        AttributeArgument.Types.Named n
    ) => AnalyzeValueRef(context, n.Value);

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
            return [];
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeAttributeRef(AnalyzerContext context, AttributeRef attrRef) =>
        attrRef.Argument.SelectMany(a => AnalyzeAttributeArgument(context.Argument(a), a));

    public virtual IEnumerable<AnalyzeError> AnalyzePropertyRef(AnalyzerContext context, PropertyRef propRef) =>
        AnalyzeTypeRef(context, propRef.Type)
            .Concat(propRef.Attributes.SelectMany(a => AnalyzeAttributeRef(context.Attribute(a), a)));

    public virtual IEnumerable<AnalyzeError> AnalyzeConstantRef(AnalyzerContext context, ConstantRef constant) =>
        AnalyzeValueRef(context, constant.Value);

    public virtual IEnumerable<AnalyzeError> AnalyzeEnumValue(AnalyzerContext context, EnumValue enumVal) => [];

    public virtual IEnumerable<AnalyzeError> AnalyzeGroupErrorCode(
        AnalyzerContext context,
        ErrorCode errCode,
        ErrorCode.Types.Group g
    ) => AnalyzeErrorCodes(context, g.InnerCodes);

    public virtual IEnumerable<AnalyzeError> AnalyzeSingleErrorCode(
        AnalyzerContext context,
        ErrorCode errCode,
        ErrorCode.Types.Single s
    ) => [];

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
            return [];
        }
    }

    public virtual IEnumerable<AnalyzeError> AnalyzeErrorCodes(
        AnalyzerContext context,
        IEnumerable<ErrorCode> errCodes
    ) => errCodes.SelectMany(e => AnalyzeErrorCode(context.Descend(e), e));

    public virtual IEnumerable<AnalyzeError> AnalyzeTypeDescriptor(AnalyzerContext context, TypeDescriptor descr) =>
        descr
            .Extends.SelectMany(t => AnalyzeTypeRef(context.Extends(t), t))
            .Concat(
                descr.GenericParameters.SelectMany((g, i) => AnalyzeGenericParameter(context.GenericParameter(i, g), g))
            )
            .Concat(descr.Properties.SelectMany(p => AnalyzePropertyRef(context.Descend(p), p)))
            .Concat(descr.Constants.SelectMany(c => AnalyzeConstantRef(context.Descend(c), c)));

    public virtual IEnumerable<AnalyzeError> AnalyzeTypeDescriptorForQuery(
        AnalyzerContext context,
        TypeDescriptor descr
    ) =>
        descr
            .Extends.Where(e => e.Known is null || e.Known.Type != KnownType.Query) // Exclude `Query` type, as it will be checked by the `Return` check
            .SelectMany(t => AnalyzeTypeRef(context.Extends(t), t))
            .Concat(
                descr.GenericParameters.SelectMany((g, i) => AnalyzeGenericParameter(context.GenericParameter(i, g), g))
            )
            .Concat(descr.Properties.SelectMany(p => AnalyzePropertyRef(context.Descend(p), p)))
            .Concat(descr.Constants.SelectMany(c => AnalyzeConstantRef(context.Descend(c), c)));

    public virtual IEnumerable<AnalyzeError> AnalyzeTypeDescriptorForOperation(
        AnalyzerContext context,
        TypeDescriptor descr
    ) =>
        descr
            .Extends.Where(e => e.Known is null || e.Known.Type != KnownType.Operation) // Exclude `Operation` type, as it will be checked by the `Return` check
            .SelectMany(t => AnalyzeTypeRef(context.Extends(t), t))
            .Concat(
                descr.GenericParameters.SelectMany((g, i) => AnalyzeGenericParameter(context.GenericParameter(i, g), g))
            )
            .Concat(descr.Properties.SelectMany(p => AnalyzePropertyRef(context.Descend(p), p)))
            .Concat(descr.Constants.SelectMany(c => AnalyzeConstantRef(context.Descend(c), c)));

    public virtual IEnumerable<AnalyzeError> AnalyzeTypeDescriptorForTopic(
        AnalyzerContext context,
        TypeDescriptor descr
    ) =>
        descr
            .Extends.Where(e => e.Known is null || e.Known.Type != KnownType.Topic)
            .SelectMany(t => AnalyzeTypeRef(context.Extends(t), t))
            .Concat(descr.Properties.SelectMany(p => AnalyzePropertyRef(context.Descend(p), p)))
            .Concat(descr.Constants.SelectMany(c => AnalyzeConstantRef(context.Descend(c), c)));

    public virtual IEnumerable<AnalyzeError> AnalyzeDTO(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.DTO dto
    ) => AnalyzeTypeDescriptor(context, dto.TypeDescriptor);

    public virtual IEnumerable<AnalyzeError> AnalyzeEnum(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Enum @enum
    ) => @enum.Members.SelectMany(m => AnalyzeEnumValue(context.Descend(m), m));

    public virtual IEnumerable<AnalyzeError> AnalyzeQuery(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Query query
    ) =>
        AnalyzeTypeDescriptorForQuery(context, query.TypeDescriptor)
            .Concat(AnalyzeTypeRef(context.Returns(query.ReturnType), query.ReturnType));

    public virtual IEnumerable<AnalyzeError> AnalyzeCommand(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Command command
    ) =>
        AnalyzeTypeDescriptor(context, command.TypeDescriptor)
            .Concat(AnalyzeErrorCodes(context.ErrorCodes(), command.ErrorCodes));

    public virtual IEnumerable<AnalyzeError> AnalyzeOperation(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Operation operation
    ) =>
        AnalyzeTypeDescriptorForOperation(context, operation.TypeDescriptor)
            .Concat(AnalyzeTypeRef(context.Returns(operation.ReturnType), operation.ReturnType));

    public virtual IEnumerable<AnalyzeError> AnalyzeTopic(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Topic topic
    ) =>
        AnalyzeTypeDescriptorForTopic(context, topic.TypeDescriptor)
            .Concat(topic.Notifications.SelectMany(n => AnalyzeTypeRef(context.Returns(n.Type), n.Type)));

    public virtual IEnumerable<AnalyzeError> AnalyzeStatement(AnalyzerContext context, Statement stmt)
    {
        return AnalyzeInner(context, stmt)
            .Concat(stmt.Attributes.SelectMany(a => AnalyzeAttributeRef(context.Attribute(a), a)));

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
            else if (stmt.Operation is Statement.Types.Operation op)
            {
                return AnalyzeOperation(context, stmt, op);
            }
            else if (stmt.Topic is Statement.Types.Topic topic)
            {
                return AnalyzeTopic(context, stmt, topic);
            }
            else
            {
                return [];
            }
        }
    }
}
