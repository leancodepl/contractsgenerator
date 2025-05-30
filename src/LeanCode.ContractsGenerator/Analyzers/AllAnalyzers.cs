namespace LeanCode.ContractsGenerator.Analyzers;

public class AllAnalyzers : IAnalyzer
{
    private readonly IReadOnlyList<IAnalyzer> analyzers =
    [
        new InternalStructureCheck(),
        new KnownTypeCheck(),
        new ErrorCodesUniqueness(),
        new ExternalTypeCheck(),
        new InvalidTypeCheck(),
        new TopicWithoutNotificationCheck(),
        new TopicWithNullableNotificationCheck(),
        new TopicMustProduceInternalType(),
    ];

    public IEnumerable<AnalyzeError> Analyze(Export export) => analyzers.SelectMany(a => a.Analyze(export));
}
