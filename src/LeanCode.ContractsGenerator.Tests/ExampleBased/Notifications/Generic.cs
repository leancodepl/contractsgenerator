using LeanCode.Contracts;
using Notifications.Generic;
using Xunit;
using static LeanCode.ContractsGenerator.Tests.NotificationTypeRefExtensions;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

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
                WithTag(
                    TypeRefExtensions
                        .Internal("Notifications.Generic.Notification1")
                        .WithArguments(Known(KnownType.Int32)),
                    NotificationTagGenerator.Generate(typeof(Notification1<int>))
                )
            );
    }

    [Fact]
    public void Multiple_generic_arguments_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.Generic.DTO1")
            .WithTopic("Notifications.Generic.Topic2")
            .WithNotification(
                WithTag(
                    TypeRefExtensions
                        .Internal("Notifications.Generic.Notification2")
                        .WithArguments(
                            Known(KnownType.Int32),
                            TypeRefExtensions.Internal("Notifications.Generic.DTO1")
                        ),
                    NotificationTagGenerator.Generate(typeof(Notification2<int, DTO1>))
                )
            );
    }

    [Fact]
    public void Nested_generic_arguments_notification()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithDto("Notifications.Generic.DTO2")
            .WithTopic("Notifications.Generic.Topic3")
            .WithNotification(
                WithTag(
                    TypeRefExtensions
                        .Internal("Notifications.Generic.Notification2")
                        .WithArguments(
                            Known(KnownType.DateTimeOffset),
                            TypeRefExtensions
                                .Internal("Notifications.Generic.DTO2")
                                .WithArguments(Known(KnownType.Int32))
                        ),
                    NotificationTagGenerator.Generate(typeof(Notification2<DateTimeOffset, DTO2<int>>))
                )
            );
    }

    [Fact]
    public void Multiple_generic_notifications_topic()
    {
        "notifications/generic.cs"
            .Compiles()
            .WithTopic("Notifications.Generic.Topic4")
            .WithNotification(
                WithTag(
                    TypeRefExtensions
                        .Internal("Notifications.Generic.Notification1")
                        .WithArguments(Known(KnownType.Int32)),
                    NotificationTagGenerator.Generate(typeof(Notification1<int>))
                )
            )
            .WithNotification(
                WithTag(
                    TypeRefExtensions
                        .Internal("Notifications.Generic.Notification2")
                        .WithArguments(Known(KnownType.Uint8), Known(KnownType.TimeSpan)),
                    NotificationTagGenerator.Generate(typeof(Notification2<byte, TimeSpan>))
                )
            );
    }
}
