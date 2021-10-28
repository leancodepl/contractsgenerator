namespace LeanCode.ContractsGenerator.Analyzers;

public class InternalStructureCheck : BaseAnalyzer
{
    public const string Code = "CNTR0001";

    public override IEnumerable<AnalyzeError> AnalyzeTypeRef(TypeRef typeRef)
    {
        if (typeRef.Generic is null &&
            typeRef.Internal is null &&
            typeRef.Known is null)
        {
            yield return new(Code, $"`{nameof(TypeRef)}` type is unknown: {typeRef}.", nameof(TypeRef), "");
        }
    }

    public override IEnumerable<AnalyzeError> AnalyzeValueRef(ValueRef vr)
    {
        if (vr.Null is null &&
            vr.Number is null &&
            vr.FloatingPoint is null &&
            vr.String is null &&
            vr.Bool is null)
        {
            yield return new(Code, $"`{nameof(ValueRef)}` type is unknown: {vr}.", nameof(ValueRef), "");
        }
    }

    public override IEnumerable<AnalyzeError> AnalyzeAttributeArgument(AttributeArgument arg)
    {
        if (arg.Positional is null && arg.Named is null)
        {
            yield return new(Code, $"`{nameof(AttributeArgument)}` type is unknown: {arg}.", nameof(AttributeArgument), "");
        }
    }

    public override IEnumerable<AnalyzeError> AnalyzeErrorCode(ErrorCode errCode)
    {
        if (errCode.Single is null && errCode.Group is null)
        {
            yield return new(Code, $"`{nameof(ErrorCode)}` type is unknown: {errCode}.", nameof(ErrorCode), "");
        }
    }

    public override IEnumerable<AnalyzeError> AnalyzeStatement(Statement stmt)
    {
        if (stmt.Dto is null &&
            stmt.Enum is null &&
            stmt.Query is null &&
            stmt.Command is null)
        {
            yield return new(Code, $"`{nameof(Statement)}` type is unknown: {stmt}.", nameof(Statement), stmt.Name);
        }
    }
}
