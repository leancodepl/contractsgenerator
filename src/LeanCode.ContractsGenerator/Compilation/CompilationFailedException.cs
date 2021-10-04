using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator.Compilation;

public class CompilationFailedException : Exception
{
    public ImmutableArray<Diagnostic> Diagnostics { get; }

    public CompilationFailedException(ImmutableArray<Diagnostic> diagnostics)
        : base("Contracts compilation failed.")
    {
        Diagnostics = diagnostics;
    }

    public CompilationFailedException(string message)
        : base(message)
    {
        Diagnostics = ImmutableArray<Diagnostic>.Empty;
    }
}
