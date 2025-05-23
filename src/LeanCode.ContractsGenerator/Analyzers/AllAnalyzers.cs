namespace LeanCode.ContractsGenerator.Analyzers;

public class AllAnalyzers(GeneratorConfiguration configuration) : IAnalyzer
{
    private readonly IReadOnlyList<IAnalyzer> analyzers =
    [
        new InternalStructureCheck(),
        new KnownTypeCheck(),
        new ErrorCodesUniqueness(),
        new ExternalTypeCheck(),
        .. configuration.AllowDateTime ? Enumerable.Empty<IAnalyzer>() : [new DateTimeTypeCheck()],
        new TopicWithoutNotificationCheck(),
        new TopicWithNullableNotificationCheck(),
        new TopicMustProduceInternalType(),
    ];

    public IEnumerable<AnalyzeError> Analyze(Export export) => analyzers.SelectMany(a => a.Analyze(export));
}
