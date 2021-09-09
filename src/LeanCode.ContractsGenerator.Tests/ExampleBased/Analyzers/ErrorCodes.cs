using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Analyzers
{
    public class ErrorCodes
    {
        [Fact]
        public void Duplicated_error_codes_in_command_are_detected()
        {
            "analyzers/error_codes.cs"
                .AnalyzeFails()
                    .WithError("LNCD003", "Cmd1")
                    .WithError("LNCD003", "Cmd2");
        }
    }
}
