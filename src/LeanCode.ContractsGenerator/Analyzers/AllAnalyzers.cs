namespace LeanCode.ContractsGenerator.Analyzers;

public class AllAnalyzers : IAnalyzer
{
    private readonly IReadOnlyList<IAnalyzer> analyzers = new IAnalyzer[]
    {
        new InternalStructureCheck(),
        new KnownTypeCheck(),
        new ErrorCodesUniqueness(),
        new ExternalTypeCheck(),
        new InvalidTypeCheck(),
        new TopicWithoutNotificationCheck(),
        new TopicWithNullableNotificationCheck(),
    };

    public IEnumerable<AnalyzeError> Analyze(Export export)
    {
        return analyzers.SelectMany(a => a.Analyze(export));
    }
}
