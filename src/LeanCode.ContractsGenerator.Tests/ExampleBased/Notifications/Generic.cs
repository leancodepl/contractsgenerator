using Notifications.Generic;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Notifications;

public class Generic
{
    [Fact]
    public void Single_generic_argument_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.Generic.Notification1")
            .WithTopic("Notifications.Generic.Topic1")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Generic.Notification1")
                        .WithArguments(TypeRefExtensions.Known(KnownType.Int32)),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(Notification1<int>))));
                    // Notifications.Generic.Notification1[!Int32]
    }

    [Fact]
    public void Multiple_generic_arguments_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.Generic.DTO1")
            .WithTopic("Notifications.Generic.Topic2")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Generic.Notification2")
                        .WithArguments(
                            TypeRefExtensions.Known(KnownType.Int32),
                            TypeRefExtensions.Internal("Notifications.Generic.DTO1")),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(Notification2<int, DTO1>))));
                    // Notifications.Generic.Notification2[!Int32,Notifications.Generic.DTO1]
    }

    [Fact]
    public void Nested_generic_arguments_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.Generic.DTO2")
            .WithTopic("Notifications.Generic.Topic3")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Generic.Notification2")
                        .WithArguments(
                            TypeRefExtensions.Known(KnownType.DateTimeOffset),
                            TypeRefExtensions
                                .Internal("Notifications.Generic.DTO2")
                                .WithArguments(TypeRefExtensions.Known(KnownType.Int32))),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(Notification2<DateTimeOffset, DTO2<int>>))));
                    // Notifications.Generic.Notification2[!DateTimeOffset,Notifications.Generic.DTO2[!Int32]]
    }

    [Fact]
    public void Nested_known_type_generic_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.Generic.DTO2")
            .WithTopic("Notifications.Generic.Topic4")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Map(
                        TypeRefExtensions.Known(KnownType.Int32),
                        TypeRefExtensions
                            .Internal("Notifications.Generic.DTO2")
                            .WithArguments(TypeRefExtensions.Known(KnownType.Int32))),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(Dictionary<int, DTO2<int>>))));
                    // !Map[!Int32,Notifications.Generic.DTO2[!Int32]]
    }

    [Fact]
    public void Multiple_generic_notifications_topic()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithTopic("Notifications.Generic.Topic5")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Generic.Notification1")
                        .WithArguments(TypeRefExtensions.Known(KnownType.Int32)),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(Notification1<int>))))
                    // !Map[!Int32,Notifications.Generic.DTO2[!Int32]]
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Generic.Notification2")
                        .WithArguments(
                            TypeRefExtensions.Known(KnownType.Uint8),
                            TypeRefExtensions.Known(KnownType.TimeSpan)),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(Notification2<byte, TimeSpan>))));
                    // Notifications.Generic.Notification2[!Uint8,!TimeSpan]
    }
}
