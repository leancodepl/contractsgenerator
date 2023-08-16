namespace LeanCode.ContractsGenerator.Analyzers;

public class KnownTypeCheck : BaseAnalyzer
{
    private static readonly IReadOnlySet<KnownType> ValidKnownTypeValues =
        Enum.GetValues<KnownType>().ToHashSet();

    public const string Code = "CNTR0002";

    public override IEnumerable<AnalyzeError> AnalyzeKnownType(
        AnalyzerContext context,
        KnownType knownType
    )
    {
        if (!ValidKnownTypeValues.Contains(knownType))
        {
            yield return new(Code, $"`KnownType` value {knownType} is unsupported.", context);
        }
    }
}
