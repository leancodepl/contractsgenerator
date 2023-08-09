using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Notifications;

public class KnownTypes
{
    [Fact]
    public void Int()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic1")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.Int32),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(int))
                )
            );
        // !Int32
    }

    [Fact]
    public void Bool()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic2")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.Boolean),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(bool))
                )
            );
        // !Boolean
    }

    [Fact]
    public void DateTimeOffset()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic3")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.DateTimeOffset),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(DateTimeOffset))
                )
            );
        // !DateTimeOffset
    }

    [Fact]
    public void Guid()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic4")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.Guid),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(Guid))
                )
            );
        // !Guid
    }

    [Fact]
    public void Array()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic5")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32)),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(int[]))
                )
            );
        // !Array[!Int32]
    }

    [Fact]
    public void Multi_dimentional_array()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic6")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Array(
                        TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32))
                    ),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(int[,,][]))
                )
            );
        // !Array[!Array[!Int32]]
    }

    [Fact]
    public void Object()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic7")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.Object),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(object))
                )
            );
        // !Object
    }

    [Fact]
    public void List()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic8")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32)),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(List<int>))
                )
            );
        // !Array[!Int32]
    }

    [Fact]
    public void String()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic9")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Known(KnownType.String),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(string))
                )
            );
        // !String
    }

    [Fact]
    public void Dictionary()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic10")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Map(
                        TypeRefExtensions.Known(KnownType.Int32),
                        TypeRefExtensions.Known(KnownType.String)
                    ),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(
                        typeof(Dictionary<int, string>)
                    )
                )
            );
        // !Map[!Int32,!String]
    }

    [Fact]
    public void IEnumerable()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic11")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32)),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(typeof(IEnumerable<int>))
                )
            );
        // !Array[!Int32]
    }

    [Fact]
    public void IDictionary()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic12")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Map(
                        TypeRefExtensions.Known(KnownType.Int32),
                        TypeRefExtensions.Known(KnownType.String)
                    ),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(
                        typeof(IDictionary<int, string>)
                    )
                )
            );
        // !Map[!Int32,!String]
    }

    [Fact]
    public void IReadOnlyDictionary()
    {
        "notifications/known_types.cs"
            .Compiles()
            .WithTopic("Notifications.KnownTypes.Topic13")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(
                    TypeRefExtensions.Map(
                        TypeRefExtensions.Known(KnownType.Int32),
                        TypeRefExtensions.Known(KnownType.String)
                    ),
                    LeanCode.Contracts.NotificationTagGenerator.Generate(
                        typeof(IReadOnlyDictionary<int, string>)
                    )
                )
            );
        // !Map[!Int32,!String]
    }
}
