namespace LeanCode.Contracts.Validation;

[method: System.Text.Json.Serialization.JsonConstructor]
public class ValidationError(string propertyName, string errorMessage, int errorCode, string? errorName)
{
    public ValidationError(string propertyName, string errorMessage, int errorCode)
        : this(propertyName, errorMessage, errorCode, null) { }

    public string PropertyName { get; } = propertyName;
    public string ErrorMessage { get; } = errorMessage;
    public int ErrorCode { get; } = errorCode;
    public string? ErrorName { get; } = errorName;
}
