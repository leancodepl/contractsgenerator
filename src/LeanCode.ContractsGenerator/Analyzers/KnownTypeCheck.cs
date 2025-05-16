namespace LeanCode.ContractsGenerator.Analyzers;

public class KnownTypeCheck : BaseAnalyzer
{
    public override IEnumerable<AnalyzeError> AnalyzeKnownType(AnalyzerContext context, KnownType knownType)
    {
        if (!Enum.IsDefined(knownType))
        {
            yield return new(
                AnalyzerCodes.UnsupportedKnownType,
                $"`KnownType` value {knownType} is unsupported.",
                context
            );
        }
    }
}
