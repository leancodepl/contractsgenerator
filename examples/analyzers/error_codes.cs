using LeanCode.Contracts;
using LeanCode.Contracts.Security;

public class Dto
{
    public abstract class ErrorCodes
    {
        public const int A = 100;
        public const int B = 101;
    }
}

[AllowUnauthorized]
public class Cmd1 : ICommand
{
    public static class ErrorCodes
    {
        public const int A = 1;
        public const int B = 1;
    }
}

[AllowUnauthorized]
public class Cmd2 : ICommand
{
    public static class ErrorCodes
    {
        public const int Dup = 100;

        public class Inner : Dto.ErrorCodes { }
    }
}
