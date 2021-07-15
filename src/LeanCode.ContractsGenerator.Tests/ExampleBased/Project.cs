using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased
{
    public class Project
    {
        [Fact]
        public void Single_project_compiles_correctly()
        {
            "project/single/single.csproj"
                .ProjectCompiles()
                .WithCommand("Single.Command");
        }

        [Fact]
        public void Aggregated_project_compiles()
        {
            "project/aggregated/Combined/Combined.csproj"
                .ProjectCompiles("project/aggregated/aggregated.sln")
                .WithCommand("A.Command")
                .WithQuery("B.Query");
        }
    }
}
