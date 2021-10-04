namespace LeanCode.ContractsGenerator.Compilation;

public class InvalidProjectException : Exception
{
    public InvalidProjectException(string msg)
        : base(msg)
    { }
}
