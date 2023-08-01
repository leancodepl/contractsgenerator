namespace LeanCode.ContractsGenerator.Analyzers;

public class InvalidTypeCheck : BaseAnalyzer
{
    public const string Code = "CNTR0006";

    public static readonly IReadOnlyDictionary<string, string> InvalidTypes = new Dictionary<string, string>
    {
        ["System.DateTime"] = "Use `DateTimeOffset` with zero offset instead.",
    };

    public override IEnumerable<AnalyzeError> AnalyzeInternalTypeRef(AnalyzerContext context, TypeRef typeRef, TypeRef.Types.Internal i)
    {
        if (InvalidTypes.TryGetValue(i.Name, out var msg))
        {
            return new[] { new AnalyzeError(Code, $"Type `{i.Name}` is unsupported. {msg}", context) };
        }
        else
        {
            return base.AnalyzeInternalTypeRef(context, typeRef, i);
        }
    }
}
