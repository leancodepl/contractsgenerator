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
                    .WithError("CNTR0003", "Cmd1.ErrorCodes")
                    .WithError("CNTR0003", "Cmd2.ErrorCodes");
        }
    }
}
