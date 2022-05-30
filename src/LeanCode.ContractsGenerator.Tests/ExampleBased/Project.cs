using Xunit;
using static LeanCode.ContractsGenerator.Tests.ExampleBasedHelpers;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased;

[CollectionDefinition(nameof(MSBuildTestCurrentDirectoryWorkaroundCollection), DisableParallelization = true)]
public class MSBuildTestCurrentDirectoryWorkaroundCollection { }

[Collection(nameof(MSBuildTestCurrentDirectoryWorkaroundCollection))]
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
    public void Project_with_implicit_usings_compiles()
    {
        "project/implicitusings/implicitusings.csproj"
            .ProjectCompiles()
            .WithDto("ImplicitUsings.DTO")
                .WithProperty("Id", Known(KnownType.Guid));
    }

    [Fact]
    public void External_package_references_are_restored_before_compilation()
    {
        foreach (var outputDir in new[] { "bin", "obj" })
        {
            var outputPath = "examples/project/packagereference/" + outputDir;

            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, recursive: true);
            }
        }

        "project/packagereference/packagereference.csproj"
            .ProjectCompiles()
            .WithDto("PackageReference.OrderDTO")
                .WithProperty("Id", Known(KnownType.Int64));
    }

    [Fact]
    public void Project_with_reference_to_assembly_with_embedded_contracts_has_them_in_the_output()
    {
        "project/referencetoembedded/referencetoembedded.csproj"
            .ProjectCompiles()
            .WithDto("Embedded.DTO")
                .WithProperty("Id", Known(KnownType.Int32));
    }
}
