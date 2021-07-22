using System;

namespace LeanCode.ContractsGenerator
{
    public class InvalidProjectException : Exception
    {
        public InvalidProjectException(string msg)
            : base(msg)
        { }
    }
}
