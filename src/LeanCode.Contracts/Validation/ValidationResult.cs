using System.Collections.Immutable;

namespace LeanCode.Contracts.Validation;

public class ValidationResult
{
    public ImmutableList<ValidationError> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    public ValidationResult(IReadOnlyList<ValidationError>? errors)
    {
        Errors = errors is null
            ? ImmutableList.Create<ValidationError>()
            : errors.ToImmutableList();
    }
}
