using System;
using Xunit;
using static LeanCode.ContractsGenerator.Tests.ExampleBasedHelpers;

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
                .ProjectCompiles()
                .WithCommand("A.Command")
                .WithQuery("B.Query");
        }

        [Fact]
        public void Multiple_separate_projects_compile()
        {
            ProjectsCompile("project/aggregated/A/A.csproj", "project/aggregated/B/B.csproj")
                .WithCommand("A.Command")
                .WithQuery("B.Query");
        }

        [Fact]
        public void Multiple_separate_projects_compile_even_if_are_totally_unrelated()
        {
            ProjectsCompile(
                "project/aggregated/A/A.csproj",
                "project/single/single.csproj")
                .WithCommand("A.Command")
                .WithCommand("Single.Command");
        }

        [Fact]
        public void Multiple_separate_projects_compile_even_if_some_references_overlap()
        {
            // The order might matter so we check a few cases
            ProjectsCompile(
                "project/aggregated/A/A.csproj",
                "project/aggregated/B/B.csproj",
                "project/aggregated/Combined/Combined.csproj")
                .WithCommand("A.Command")
                .WithQuery("B.Query");

            ProjectsCompile(
                "project/aggregated/Combined/Combined.csproj",
                "project/aggregated/A/A.csproj",
                "project/aggregated/B/B.csproj")
                .WithCommand("A.Command")
                .WithQuery("B.Query");
        }

        [Fact]
        public void Globbing_finds_necessary_files()
        {
            GlobCompiles(
                includes: new[] { "project/globs/**/*.cs", },
                excludes: Array.Empty<string>())
                .WithCommand("A.Command")
                .WithQuery("B.Query")
                .WithDto("A.Dto");
        }

        [Fact]
        public void Globbing_includes_correct_files()
        {
            GlobCompiles(
                includes: new[] { "project/globs/A/*", },
                excludes: Array.Empty<string>())
                .WithCommand("A.Command")
                .WithDto("A.Dto")
                .Without("B.Query");
        }

        [Fact]
        public void Globbing_excludes_correct_files()
        {
            GlobCompiles(
                includes: new[] { "project/globs/**", },
                excludes: new[] { "**/Dto.cs" })
                .WithCommand("A.Command")
                .Without("A.Dto")
                .WithQuery("B.Query");
        }
    }
}
