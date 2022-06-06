using LeanCode.Contracts;

public class DTO
{
    public class ErrorCodes
    {
        public const int DtoRelatedError = 1_000;
    }
}

public class Command : ICommand
{
    public DTO NeededDTO { get; set; } = default!;

    public class ErrorCodes
    {
        public sealed class DTOErrors : DTO.ErrorCodes { }

        public const int CommandSpecificError = 1;
    }
}
