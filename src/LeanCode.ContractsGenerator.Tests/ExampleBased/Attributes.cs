using Xunit;
using static LeanCode.ContractsGenerator.Tests.AttributeArgumentExtensions;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased
{
    public class Attributes
    {
        [Fact]
        public void Property_attributes_are_preserved()
        {
            "attributes/property.cs"
                .Compiles()
                .WithCommand("A")
                    .WithProperty(
                        "Prop",
                        p => p
                            .WithAttribute("System.ObsoleteAttribute", Positional(0, "Msg")));
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
                    .WithAttribute("LeanCode.CQRS.Security.AllowUnauthorizedAttribute");
        }
    }
}
