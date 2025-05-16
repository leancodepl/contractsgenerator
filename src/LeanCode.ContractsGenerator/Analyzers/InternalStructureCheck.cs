namespace LeanCode.ContractsGenerator.Analyzers;

public class InternalStructureCheck : BaseAnalyzer
{
    public const string Code = AnalyzerCodes.InternalStructureIsWrong;

    public override IEnumerable<AnalyzeError> AnalyzeTypeRef(AnalyzerContext context, TypeRef typeRef)
    {
        if (typeRef.Generic is null && typeRef.Internal is null && typeRef.Known is null)
        {
            yield return new(Code, $"`{nameof(TypeRef)}` type is unknown: {typeRef}.", context);
        }
    }

    public override IEnumerable<AnalyzeError> AnalyzeValueRef(AnalyzerContext context, ValueRef valueRef)
    {
        if (
            valueRef.Null is null
            && valueRef.Number is null
            && valueRef.FloatingPoint is null
            && valueRef.String is null
            && valueRef.Bool is null
        )
        {
            yield return new(Code, $"`{nameof(ValueRef)}` type is unknown: {valueRef}.", context);
        }
    }

    public override IEnumerable<AnalyzeError> AnalyzeAttributeArgument(AnalyzerContext context, AttributeArgument arg)
    {
        if (arg.Positional is null && arg.Named is null)
        {
            yield return new(Code, $"`{nameof(AttributeArgument)}` type is unknown: {arg}.", context);
        }
    }

    public override IEnumerable<AnalyzeError> AnalyzeErrorCode(AnalyzerContext context, ErrorCode errCode)
    {
        if (errCode.Single is null && errCode.Group is null)
        {
            yield return new(Code, $"`{nameof(ErrorCode)}` type is unknown: {errCode}.", context);
        }
    }

    public override IEnumerable<AnalyzeError> AnalyzeStatement(AnalyzerContext context, Statement stmt)
    {
        if (
            stmt.Dto is null
            && stmt.Enum is null
            && stmt.Query is null
            && stmt.Command is null
            && stmt.Operation is null
            && stmt.Topic is null
        )
        {
            yield return new(Code, $"`{nameof(Statement)}` type is unknown: {stmt}.", context);
        }
    }
}
