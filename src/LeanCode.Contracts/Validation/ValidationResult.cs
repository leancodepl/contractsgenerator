using System.Collections.Immutable;

namespace LeanCode.Contracts.Validation;

public class ValidationResult(IReadOnlyList<ValidationError>? errors)
{
    public ImmutableList<ValidationError> Errors { get; } = errors is null ? [] : [.. errors];
    public bool IsValid => Errors.Count == 0;
}
