using Notifications.Internal;
using Xunit;
using LeanCode.Contracts;
using static LeanCode.ContractsGenerator.Tests.NotificationTypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Notifications;

public class Internal
{
    [Fact]
    public void Internal_notifications()
    {
        "notifications/internal.cs"
            .Compiles()
            .WithTopic("Notifications.Internal.Topic")
            .WithNotification(
                WithTag(
                    TypeRefExtensions.Internal("Notifications.Internal.DTO1"),
                    NotificationTagGenerator.Generate(typeof(DTO1))
                )
            )
            .WithNotification(
                WithTag(
                    TypeRefExtensions.Internal("Notifications.Internal.DTO2"),
                    NotificationTagGenerator.Generate(typeof(DTO2))
                )
            );
    }
}
