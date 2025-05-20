namespace LeanCode.ContractsGenerator;

public class AnalyzeFailedException(IReadOnlyList<AnalyzeError> errors)
    : Exception(string.Join('\n', errors.Select(e => e.ToString()).Prepend("Analyze phase failed.")))
{
    public IReadOnlyList<AnalyzeError> Errors { get; } = errors;
}
