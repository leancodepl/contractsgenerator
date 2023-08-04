using Notifications.Internal;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Notifications;

public class Internal
{
    [Fact]
    public void Internal_notifications()
    {
        "notifications/internal.cs"
            .Compiles()
            .WithDto("Notifications.Internal.DTO1")
            .WithDto("Notifications.Internal.DTO2")
            .WithTopic("Notifications.Internal.Topic")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Internal.DTO1"),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(DTO1))))
                    // Notifications.Internal.DTO1
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Internal("Notifications.Internal.DTO2"),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(DTO2))));
                    // Notifications.Internal.DTO2
    }
}
