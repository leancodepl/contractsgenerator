using System;

namespace LeanCode.ContractsGenerator
{
    public class GenerationFailedException : Exception
    {
        public GenerationFailedException(string message)
            : base(message)
        { }
    }
}
