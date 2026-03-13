namespace LeanCode.Contracts.Validation;

public class ValidationError(string propertyName, string errorMessage, int errorCode, string? errorCodeName = null)
{
    public string PropertyName { get; } = propertyName;
    public string ErrorMessage { get; } = errorMessage;
    public int ErrorCode { get; } = errorCode;
    public string? ErrorCodeName { get; } = errorCodeName;
}
