namespace LeanCode.ContractsGeneratorV2.ExampleContracts.Users
{
    public class UserInfoDTO
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }

        public class ErrorCodes
        {
            public const int FirstnameIsNull = 1001;
            public const int SurnameIsNull = 1002;
            public const int UsernameIsNull = 1003;
            public const int FirstnameTooLong = 1004;
            public const int SurnameTooLong = 1005;
            public const int UsernameTooLong = 1006;
            public const int EmailAddressTooLong = 1007;
            public const int InvalidEmailAddress = 1009;
            public const int EmailIsTaken = 1010;
        }
    }
}