using Xunit;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased;

public class Simple
{
    [Fact]
    public void Simple_command()
    {
        "simple/command.cs".Compiles().WithSingle().Command("Command").ThatExtends(Known(KnownType.Command));
    }

    [Fact]
    public void Simple_query()
    {
        "simple/query.cs"
            .Compiles()
            .WithSingle()
            .Query("Query")
            .WithReturnType(Known(KnownType.Int32))
            .ThatExtends(Known(KnownType.Query).WithArguments(Known(KnownType.Int32)));
    }

    [Fact]
    public void Simple_operation()
    {
        "simple/operation.cs"
            .Compiles()
            .WithSingle()
            .Operation("Operation")
            .WithReturnType(Known(KnownType.Int32))
            .ThatExtends(Known(KnownType.Operation).WithArguments(Known(KnownType.Int32)));
    }

    [Fact]
    public void Simple_Dto()
    {
        "simple/dto.cs".Compiles().WithSingle().Dto("DTO");
    }

    [Fact]
    public void Simple_Topic()
    {
        "simple/topic.cs"
            .Compiles()
            .WithDto("Notification")
            .WithTopic("Topic")
            .WithNotification(
                NotificationTypeRefExtensions.WithTag(TypeRefExtensions.Internal("Notification"), "Notification")
            );
    }

    [Fact]
    public void Simple_Struct()
    {
        "simple/struct.cs".Compiles().WithSingle().Dto("DTO");
    }

    [Fact]
    public void Simple_Record()
    {
        "simple/record.cs".Compiles().WithSingle().Dto("DTO");
    }

    [Fact]
    public void Record_Struct()
    {
        "simple/record_struct.cs".Compiles().WithSingle().Dto("DTO");
    }

    [Fact]
    public void Record_Class()
    {
        "simple/record_class.cs".Compiles().WithSingle().Dto("DTO");
    }

    [Fact]
    public void Simple_Enum()
    {
        "simple/enum.cs"
            .Compiles()
            .WithSingle()
            .Enum("SimpleEnum")
            .WithMember("Z", -1)
            .WithMember("A", 0)
            .WithMember("B", 1)
            .WithMember("C", 10);
    }

    [Fact]
    public void Inherited_properties()
    {
        "simple/inheritance.cs"
            .Compiles()
            .WithDto("A")
            .WithProperty("PropA", Known(KnownType.Int32))
            .WithDto("B")
            .WithProperty("PropB", Known(KnownType.Int32))
            .WithDto("C")
            .WithProperty("PropC", Known(KnownType.Int32))
            .WithoutProperty("PropA")
            .WithoutProperty("PropB")
            .WithDto("D")
            .WithProperty("PropD", Known(KnownType.Int32))
            .WithDto("E")
            .WithProperty("PropE", Known(KnownType.Int32))
            .WithoutProperty("PropD");
    }

    [Fact]
    public void Comments()
    {
        "simple/comments.cs"
            .Compiles()
            .WithQuery("Query1")
            .Commented("Test comment.")
            .WithQuery("Query2")
            .Commented("");
    }
}
