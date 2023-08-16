namespace LeanCode.ContractsGenerator.Generation;

public class GenerationFailedException : Exception
{
    public GenerationFailedException(string message)
        : base(message) { }
}
