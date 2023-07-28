using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Notifications;

public class KnownTypes
{
    [Fact]
    public void Int()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic1")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.Int32),
                    typeof(int).ToString()));
    }

    [Fact]
    public void Bool()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic2")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.Boolean),
                    typeof(bool).ToString()));
    }

    [Fact]
    public void DateTimeOffset()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic3")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.DateTimeOffset),
                    typeof(DateTimeOffset).ToString()));
    }

    [Fact]
    public void Guid()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic4")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.Guid),
                    typeof(Guid).ToString()));
    }

    [Fact]
    public void Array()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic5")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32)),
                    typeof(int[]).ToString()));
    }

    [Fact]
    public void Multi_dimentional_array()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic6")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Array(TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32))),
                    typeof(int[,,][]).ToString()));
    }

    [Fact]
    public void Object()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic7")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.Object),
                    typeof(object).ToString()));
    }

    [Fact]
    public void List()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic8")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32)),
                    typeof(List<int>).ToString()));
    }

    [Fact]
    public void String()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic9")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.String),
                    typeof(string).ToString()));
    }

    [Fact]
    public void Dictionary()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.Topic10")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Map(
                        TypeRefExtensions.Known(KnownType.Int32),
                        TypeRefExtensions.Known(KnownType.String)),
                    typeof(Dictionary<int, string>).ToString()));
    }
}
