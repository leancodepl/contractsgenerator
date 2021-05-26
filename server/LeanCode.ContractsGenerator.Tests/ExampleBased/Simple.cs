using Xunit;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased
{
    public class Simple
    {
        [Fact]
        public void Simple_command()
        {
            @"public class Command : IRemoteCommand {}"
                .Compiles()
                .WithSingle()
                .Command("Command")
                .ThatExtends(Known(KnownType.Command));
        }

        [Fact]
        public void Simple_query()
        {
            @"public class Query : IRemoteQuery<int> {}"
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
            @"public class DTO {}"
                .Compiles()
                .WithSingle()
                .Dto("DTO");
        }

        [Fact]
        public void Simple_Enum()
        {
            @"public enum SimpleEnum { A, B, C = 10 }"
                .Compiles()
                .WithSingle()
                .Enum("SimpleEnum")
                    .WithMember("A", 0)
                    .WithMember("B", 1)
                    .WithMember("C", 10);
        }
    }
}
