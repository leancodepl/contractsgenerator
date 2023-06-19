using Xunit;
using static LeanCode.ContractsGenerator.Tests.ErrorCodeExtensions;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased;

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
                .WithProperty("NeededDTO", TypeRefExtensions.Internal("DTO"))
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
            .WithStatements(2)
            .WithDto("Exclusions.IncludedDTO")
                .WithProperty("IncludedProperty", Known(KnownType.Int32))
                .WithoutProperty("ExcludedProperty")
            .WithEnum("Exclusions.IncludedEnum")
                .WithMember("IncludedValue", 0)
                .WithoutMember("ExcludedValue")
            .Without("ExcludedEnum")
            .Without("ExcludedStruct")
            .Without("ExcludedClass")
            .Without("IExcludedInterface");
    }

    [Fact]
    public void Basic_LeanPipe_setup()
    {
        "supported_use_cases/leanpipe.cs"
            .Compiles()
            .WithDto("Notification")
            .WithTopic("Topic")
            .WithNotification(TypeRefExtensions.Internal("Notification"));
    }
}
