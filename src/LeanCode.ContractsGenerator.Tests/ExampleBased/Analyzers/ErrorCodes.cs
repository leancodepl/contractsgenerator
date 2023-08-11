using LeanCode.ContractsGenerator.Analyzers;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers;

public class ErrorCodes
{
    [Fact]
    public void Duplicated_error_codes_in_command_are_detected()
    {
        "analyzers/error_codes.cs"
            .AnalyzeFails()
            .WithError(AnalyzerCodes.DuplicateErrorCodes, "Cmd1.ErrorCodes")
            .WithError(AnalyzerCodes.DuplicateErrorCodes, "Cmd2.ErrorCodes");
    }
}
