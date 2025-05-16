using System.Collections.Immutable;
using LeanCode.Contracts.Validation;

namespace LeanCode.Contracts;

public sealed class CommandResult(ImmutableList<ValidationError> validationErrors)
{
    public ImmutableList<ValidationError> ValidationErrors { get; } = validationErrors;
    public bool WasSuccessful => ValidationErrors.Count == 0;

    public static CommandResult Success { get; } = new([]);

    public static CommandResult NotValid(ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new ArgumentException(
                "Cannot create NotValid command result if no validation errors have occurred.",
                nameof(validationResult)
            );
        }

        return new(validationResult.Errors);
    }
}
