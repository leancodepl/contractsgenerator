using System;
using System.Collections.Generic;
using LeanCode.CQRS;
using LeanCode.CQRS.Security;

namespace LeanCode.ContractsGeneratorV2.ExampleContracts.Users
{
    [AuthorizeWhenHasAnyOf(Auth.Roles.Admin)]
    [Security.AuthorizeWhenHasSomethingAccess]
    public class EditUser : IRemoteCommand, Security.ISomethingRelated
    {
        public Guid UserId { get; set; }
        public Guid SomethingId { get; set; }

        public List<int> List { get; set; }
        public int[] Array { get; set; }
        public Dictionary<int, EditUser> Dictionary { get; set; }

        public UserInfoDTO UserInfo { get; set; }

        public static class ErrorCodes
        {
            public const int UserDoesNotExist = 1;
            public const int UserInfoIsNull = 2;

            public sealed class UserInfo : UserInfoDTO.ErrorCodes { }
        }
    }
}