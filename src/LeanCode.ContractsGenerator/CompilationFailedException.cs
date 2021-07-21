using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator
{
    public class CompilationFailedException : Exception
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public CompilationFailedException(ImmutableArray<Diagnostic> diagnostics)
            : base("Contracts compilation failed.")
        {
            Diagnostics = diagnostics;
        }
    }
}
