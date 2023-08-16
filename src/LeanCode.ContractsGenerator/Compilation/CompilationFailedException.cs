using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator.Compilation;

public class CompilationFailedException : Exception
{
    public ImmutableArray<Diagnostic> Diagnostics { get; }

    public CompilationFailedException(ImmutableArray<Diagnostic> diagnostics)
        : base("Contracts compilation failed. Errors:\n" + GetDiagnosticsMessage(diagnostics))
    {
        Diagnostics = diagnostics;
    }

    public CompilationFailedException(string message)
        : base(message)
    {
        Diagnostics = ImmutableArray<Diagnostic>.Empty;
    }

    private static string GetDiagnosticsMessage(ImmutableArray<Diagnostic> diagnostics)
    {
        var sb = new StringBuilder();
        foreach (var d in diagnostics)
        {
            sb.AppendLine($"[{d.Severity}] {d.GetMessage()} at {FormatLocation(d.Location)}");
        }

        return sb.ToString();
    }

    private static string FormatLocation(Location location)
    {
        var lineSpan = location.GetMappedLineSpan();
        if (lineSpan.Path is not null)
        {
            return lineSpan.Path
                + "@"
                + (lineSpan.StartLinePosition.Line + 1)
                + ":"
                + (lineSpan.StartLinePosition.Character + 1);
        }
        else if (location.IsInSource)
        {
            return location.Kind
                + "("
                + location.SourceTree?.FilePath
                + location.SourceSpan.ToString()
                + ")";
        }
        else if (location.IsInMetadata && location.MetadataModule is not null)
        {
            return location.Kind + "(" + location.MetadataModule.Name + ")";
        }
        else
        {
            return location.Kind.ToString();
        }
    }
}
