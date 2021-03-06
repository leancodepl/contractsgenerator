namespace LeanCode.ContractsGenerator;

public interface IAnalyzer
{
    IEnumerable<AnalyzeError> Analyze(Export export);
}

public record AnalyzeError(string Code, string Message, AnalyzerContext Context);
