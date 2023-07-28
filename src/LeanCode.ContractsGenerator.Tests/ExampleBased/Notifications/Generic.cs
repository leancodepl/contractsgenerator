using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Notifications;

public class Generic
{
    [Fact]
    public void Single_generic_argument_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.Notification1")
            .WithTopic("Notifications.Topic1")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Notification1")
                        .WithArguments(TypeRefExtensions.Known(KnownType.Int32)),
                    "Notifications.Notification1`1[System.Int32]"));
    }

    [Fact]
    public void Multiple_generic_arguments_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.DTO1")
            .WithTopic("Notifications.Topic2")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Notification2")
                        .WithArguments(
                            TypeRefExtensions.Known(KnownType.Int32),
                            TypeRefExtensions.Internal("Notifications.DTO1")),
                    "Notifications.Notification2`2[System.Int32,Notifications.DTO1]"));
    }

    [Fact]
    public void Nested_generic_arguments_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.DTO2")
            .WithTopic("Notifications.Topic3")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Notification2")
                        .WithArguments(
                            TypeRefExtensions.Known(KnownType.DateTimeOffset),
                            TypeRefExtensions
                                .Internal("Notifications.DTO2")
                                .WithArguments(TypeRefExtensions.Known(KnownType.Int32))),
                    "Notifications.Notification2`2[System.DateTimeOffset,Notifications.DTO2`1[System.Int32]]"));
    }

    [Fact]
    public void Nested_known_type_generic()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.DTO2")
            .WithTopic("Notifications.Topic4")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Map(
                        TypeRefExtensions.Known(KnownType.Int32),
                        TypeRefExtensions
                            .Internal("Notifications.DTO2")
                            .WithArguments(TypeRefExtensions.Known(KnownType.Int32))),
                    "System.Collections.Generic.Dictionary`2[System.Int32,Notifications.DTO2`1[System.Int32]]"));
    }
}
