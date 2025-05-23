namespace LeanCode.ContractsGenerator.Analyzers;

public abstract class InvalidInternalTypeCheck(string name, string message) : BaseAnalyzer
{
    private readonly string name = name;
    private readonly string message = message;

    public override IEnumerable<AnalyzeError> AnalyzeInternalTypeRef(
        AnalyzerContext context,
        TypeRef typeRef,
        TypeRef.Types.Internal i
    )
    {
        return string.Equals(i.Name, name, StringComparison.InvariantCulture)
            ? [new AnalyzeError(AnalyzerCodes.UnsupportedType, $"Type `{i.Name}` is unsupported. {message}", context)]
            : base.AnalyzeInternalTypeRef(context, typeRef, i);
    }
}

public abstract class InvalidKnownTypeCheck(KnownType type, string message) : BaseAnalyzer
{
    private readonly KnownType type = type;
    private readonly string message = message;

    public override IEnumerable<AnalyzeError> AnalyzeKnownTypeRef(
        AnalyzerContext context,
        TypeRef typeRef,
        TypeRef.Types.Known k
    )
    {
        return k.Type == type
            ? [new AnalyzeError(AnalyzerCodes.UnsupportedType, $"Type `{type}` is unsupported. {message}", context)]
            : base.AnalyzeKnownTypeRef(context, typeRef, k);
    }
}

public class DateTimeTypeCheck()
    : InvalidKnownTypeCheck(KnownType.DateTime, "Use `DateTimeOffset` with zero offset instead.");
