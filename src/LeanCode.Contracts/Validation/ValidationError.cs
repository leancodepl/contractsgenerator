namespace LeanCode.Contracts.Validation;

public class ValidationError(string propertyName, string errorMessage, int errorCode)
{
    public string PropertyName { get; } = propertyName;
    public string ErrorMessage { get; } = errorMessage;
    public int ErrorCode { get; } = errorCode;
}
