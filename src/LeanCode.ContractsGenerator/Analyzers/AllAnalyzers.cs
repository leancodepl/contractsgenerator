namespace LeanCode.ContractsGenerator.Analyzers;

public class AllAnalyzers : IAnalyzer
{
    private static readonly IReadOnlyList<IAnalyzer> Analyzers = new IAnalyzer[]
    {
            new InternalStructureCheck(),
            new KnownTypeCheck(),
            new ErrorCodesUniqueness(),
    };

    public IEnumerable<AnalyzeError> Analyze(Export export)
    {
        return Analyzers.SelectMany(a => a.Analyze(export));
    }
}
