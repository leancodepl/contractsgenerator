using Xunit;
using static LeanCode.ContractsGenerator.Tests.ExampleBasedHelpers;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased;

public class Glob
{
    [Fact]
    public void Globbing_finds_necessary_files()
    {
        GlobCompiles(includes: ["project/globs/**/*.cs"], excludes: [])
            .WithCommand("A.Command")
            .WithQuery("B.Query")
            .WithDto("A.Dto");
    }

    [Fact]
    public void Globbing_includes_correct_files()
    {
        GlobCompiles(includes: ["project/globs/A/*"], excludes: [])
            .WithCommand("A.Command")
            .WithDto("A.Dto")
            .Without("B.Query");
    }

    [Fact]
    public void Globbing_excludes_correct_files()
    {
        GlobCompiles(includes: ["project/globs/**"], excludes: ["**/Dto.cs"])
            .WithCommand("A.Command")
            .Without("A.Dto")
            .WithQuery("B.Query");
    }
}
