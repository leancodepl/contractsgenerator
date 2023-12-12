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
            .WithErrorCode(Group("DTOErrors", "DTO.ErrorCodes", Single("DtoRelatedError", 1_000)));
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
        "supported_use_cases/leanpipe/topic_with_parameter.cs"
            .Compiles()
            .WithDto("Notification")
            .WithProperty("Num", Known(KnownType.Int32))
            .WithTopic("Topic")
            .WithProperty("Key", Known(KnownType.String))
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notification"),
                    "Notification"
                )
            );
    }

    [Fact]
    public void Topic_with_multiple_notifications()
    {
        "supported_use_cases/leanpipe/topic_with_multiple_notifications.cs"
            .Compiles()
            .WithDto("FirstNotification")
            .WithDto("SecondNotification")
            .WithTopic("Topic")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("FirstNotification"),
                    "FirstNotification"
                )
            )
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("SecondNotification"),
                    "SecondNotification"
                )
            );
    }

    [Fact]
    public void Topic_from_abstract_base_class()
    {
        "supported_use_cases/leanpipe/topic_from_abstract_base_class.cs"
            .Compiles()
            .WithDto("Notification")
            .WithDto("TopicBase")
            .WithTopic("Topic")
            .ThatExtends(TypeRefExtensions.Internal("TopicBase"))
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notification"),
                    "Notification"
                )
            );
    }

    [Fact]
    public void Topic_from_concrete_base_class()
    {
        "supported_use_cases/leanpipe/topic_from_concrete_base_class.cs"
            .Compiles()
            .WithDto("Notification")
            .WithTopic("TopicBase")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notification"),
                    "Notification"
                )
            )
            .WithTopic("Topic")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notification"),
                    "Notification"
                )
            );
    }

    [Fact]
    public void Topic_from_base_interface()
    {
        "supported_use_cases/leanpipe/topic_from_base_interface.cs"
            .Compiles()
            .WithDto("Notification")
            .WithDto("ITopicBase")
            .WithTopic("Topic")
            .ThatExtends(TypeRefExtensions.Internal("ITopicBase"))
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notification"),
                    "Notification"
                )
            );
    }

    [Fact]
    public void Produce_notification_from_inherited_interface()
    {
        "supported_use_cases/leanpipe/produce_notification_from_inherited_interface.cs"
            .Compiles()
            .WithDto("Notification")
            .WithDto("IProducer")
            .WithTopic("Topic")
            .ThatExtends(TypeRefExtensions.Internal("IProducer"))
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notification"),
                    "Notification"
                )
            );
    }

    [Fact]
    public void Produce_notification_from_inherited_type()
    {
        "supported_use_cases/leanpipe/produce_notification_from_inherited_type.cs"
            .Compiles()
            .WithDto("Notification")
            .WithDto("Producer")
            .WithTopic("Topic")
            .ThatExtends(TypeRefExtensions.Internal("Producer"))
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notification"),
                    "Notification"
                )
            );
    }

    [Fact]
    public void Records_in_cqrs()
    {
        "supported_use_cases/records_in_cqrs.cs"
            .Compiles()
            .WithDto("DTO1")
            .WithDto("DTO2")
            .WithQuery("Query")
            .WithCommand("Command")
            .WithOperation("Operation");
    }
}
