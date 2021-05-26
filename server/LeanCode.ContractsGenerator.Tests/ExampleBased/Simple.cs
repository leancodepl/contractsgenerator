using Xunit;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased
{
    public class Simple
    {
        [Fact]
        public void Simple_command()
        {
            "simple/command.cs"
                .Compiles()
                .WithSingle()
                .Command("Command")
                .ThatExtends(Known(KnownType.Command));
        }

        [Fact]
        public void Simple_query()
        {
            "simple/query.cs"
                .Compiles()
                .WithSingle()
                .Query("Query")
                    .WithReturnType(Known(KnownType.Int32))
                    .ThatExtends(
                        Known(KnownType.Query)
                            .WithArgument(Known(KnownType.Int32)));
        }

        [Fact]
        public void Simple_Dto()
        {
            "simple/dto.cs"
                .Compiles()
                .WithSingle()
                .Dto("DTO");
        }

        [Fact]
        public void Simple_Enum()
        {
            "simple/enum.cs"
                .Compiles()
                .WithSingle()
                .Enum("SimpleEnum")
                    .WithMember("A", 0)
                    .WithMember("B", 1)
                    .WithMember("C", 10);
        }
    }
}
