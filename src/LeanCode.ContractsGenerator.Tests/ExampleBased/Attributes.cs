using Xunit;
using static LeanCode.ContractsGenerator.Tests.AttributeArgumentExtensions;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased;

public class Attributes
{
    [Fact]
    public void Property_attributes_are_preserved()
    {
        "attributes/property.cs"
            .Compiles()
            .WithCommand("A")
            .WithProperty("Prop", p => p.WithAttribute("System.ObsoleteAttribute", Positional(0, "Msg")));
    }

    [Fact]
    public void Obsolete_attribute_on_a_class_is_preserved()
    {
        "attributes/obsolete.cs"
            .Compiles()
            .WithCommand("A")
            .WithAttribute("System.ObsoleteAttribute", Positional(0, "Msg"));
    }

    [Fact]
    public void Attributes_on_enums_is_preserved()
    {
        "attributes/enum.cs"
            .Compiles()
            .WithEnum("EnumDTO")
            .WithAttribute("System.ObsoleteAttribute", Positional(0, "OnEnum"))
            .WithMember("A", 0)
            .WithMember("B", 1)
            .WithAttribute("System.ObsoleteAttribute", Positional(0, "OnMember"));
    }

    [Fact]
    public void Named_arguments_are_preserved_along_custom_attributes()
    {
        "attributes/custom.cs"
            .Compiles()
            .WithDto("CustomAttribute")
            .ThatExtends(Known(KnownType.Attribute))
            .WithDto("Dto")
            .WithAttribute("CustomAttribute", Named("NamedArg", "Test"));
    }

    [Fact]
    public void AllowUnauthorized_attribute_is_correctly_propagated()
    {
        "attributes/unauthorized.cs"
            .Compiles()
            .WithCommand("A")
            .WithAttribute("LeanCode.Contracts.Security.AllowUnauthorizedAttribute");
    }

    [Fact]
    public void AuthorizeWhen_attribute_is_correctly_propagated()
    {
        "attributes/authorize_when.cs"
            .Compiles()
            .WithCommand("A")
            .WithAttribute("AuthorizeWhenCustomCtorAttribute")
            .WithCommand("B")
            .WithAttribute("AuthorizeWhenCustomGenericAttribute")
            .WithDto("AuthorizeWhenCustomCtorAttribute")
            .ThatExtends(Known(KnownType.AuthorizeWhenAttribute))
            .WithDto("AuthorizeWhenCustomGenericAttribute")
            .ThatExtends(Known(KnownType.AuthorizeWhenAttribute));
    }
}
