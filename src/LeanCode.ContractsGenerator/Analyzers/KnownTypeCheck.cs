namespace LeanCode.ContractsGenerator.Analyzers;

public class KnownTypeCheck : BaseAnalyzer
{
    private static readonly IReadOnlySet<KnownType> ValidKnownTypeValues =
        Enum.GetValues<KnownType>().ToHashSet();

    public override IEnumerable<AnalyzeError> AnalyzeKnownType(
        AnalyzerContext context,
        KnownType knownType
    )
    {
        if (!ValidKnownTypeValues.Contains(knownType))
        {
            yield return new(
                AnalyzerCodes.UnsupportedKnownType,
                $"`KnownType` value {knownType} is unsupported.",
                context
            );
        }
    }
}
