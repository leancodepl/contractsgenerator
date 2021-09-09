namespace LeanCode.ContractsGenerator
{
    public class AnalyzeFailedException : Exception
    {
        public IReadOnlyList<AnalyzeError> Errors { get; }

        public AnalyzeFailedException(IReadOnlyList<AnalyzeError> errors)
            : base("Analyze phase failed.")
        {
            Errors = errors;
        }
    }
}
