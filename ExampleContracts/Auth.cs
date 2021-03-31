namespace LeanCode.ContractsGeneratorV2.ExampleContracts
{
    public static class Auth
    {
        public static class Roles
        {
            public const string User = "user";
            public const string Admin = "admin";
            public const string System = "system";
        }

        public static class KnownClaims
        {
            public const string UserId = "sub";
            public const string Role = "role";
        }
    }
}