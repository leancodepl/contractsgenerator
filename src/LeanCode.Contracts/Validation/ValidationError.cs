namespace LeanCode.Contracts.Validation;

public class ValidationError(string propertyName, string errorMessage, int errorCode, string? errorName = null)
{
    public string PropertyName { get; } = propertyName;
    public string ErrorMessage { get; } = errorMessage;
    public int ErrorCode { get; } = errorCode;
    public string? ErrorName { get; } = errorName;
}
