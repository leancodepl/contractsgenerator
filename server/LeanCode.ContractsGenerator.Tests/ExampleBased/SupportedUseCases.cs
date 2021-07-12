using Xunit;
using static LeanCode.ContractsGenerator.Tests.ErrorCodeExtensions;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased
{
    public class SupportedUseCases
    {
        [Fact]
        public void Same_names_across_namespaces()
        {
            "supported_use_cases/same_names.cs"
                .Compiles()
                .WithDto("First.DTO")
                .WithDto("Second.DTO")
                .WithCommand("First.Command")
                .WithCommand("Second.Command")
                .WithQuery("First.Query")
                .WithQuery("Second.Query");
        }

        [Fact]
        public void Shared_error_codes()
        {
            "supported_use_cases/shared_error_codes.cs"
                .Compiles()
                .WithDto("DTO")
                .WithCommand("Command")
                    .WithProperty("NeededDTO", Internal("DTO"))
                    .WithErrorCode(Single("CommandSpecificError", 1))
                    .WithErrorCode(
                        Group(
                            "DTOErrors",
                            "DTO.ErrorCodes",
                            Single("DtoRelatedError", 1_000)));
        }

        [Fact]
        public void Excluded_types_and_properties()
        {
            "supported_use_cases/exclusions.cs"
                .Compiles()
                .WithSingle()
                .Dto("Exclusions.IncludedDTO")
                    .WithProperty("IncludedProperty", Known(KnownType.Int32))
                    .WithoutProperty("ExcludedProperty");
        }
    }
}
