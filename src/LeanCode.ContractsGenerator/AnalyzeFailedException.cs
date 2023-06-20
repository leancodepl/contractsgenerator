namespace LeanCode.ContractsGenerator;

public class AnalyzeFailedException : Exception
{
    public IReadOnlyList<AnalyzeError> Errors { get; }

    public AnalyzeFailedException(IReadOnlyList<AnalyzeError> errors)
        : base(string.Join('\n', errors.Select(e => e.ToString()).Prepend("Analyze phase failed.")))
    {
        Errors = errors;
    }
}
