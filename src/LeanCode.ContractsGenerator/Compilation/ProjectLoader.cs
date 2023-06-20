using LeanCode.ContractsGenerator.Compilation.MSBuild;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

namespace LeanCode.ContractsGenerator.Compilation;

public sealed class ProjectLoader : IDisposable
{
    private readonly CSharpCompilationOptions options;

    private readonly MSBuildWorkspace msbuildWorkspace = MSBuildHelper.CreateWorkspace();
    private readonly List<Project> projects = new();

    public ProjectLoader(CSharpCompilationOptions options)
    {
        this.options = options;
    }

    public async Task LoadProjectsAsync(IEnumerable<string> projectPaths)
    {
        var projectPathsList = projectPaths.Select(ResolveCanonicalPath).ToList();

        if (MSBuildHelper.RestoreProjects(projectPathsList) > 0)
        {
            await Console.Error.WriteLineAsync(
                "Failed to restore some of the projects, restore them manually or expect problems.");
        }

        foreach (var projectPath in projectPathsList)
        {
            if (msbuildWorkspace.CurrentSolution.Projects
                .Select(p => p.FilePath)
                .OfType<string>()
                .Select(ResolveCanonicalPath)
                .Contains(projectPath))
            {
                continue;
            }

            var project = await msbuildWorkspace.OpenProjectAsync(projectPath);

            projects.Add(project);
        }

        static string ResolveCanonicalPath(string path)
        {
            var fileInfo = new FileInfo(path) as FileSystemInfo;

            if (!fileInfo.Exists)
            {
                return path;
            }

            fileInfo = fileInfo.ResolveLinkTarget(true) ?? fileInfo;

            if (!fileInfo.Exists)
            {
                return path;
            }

            return Path.GetFullPath(fileInfo.FullName);
        }
    }

    public async Task<IReadOnlyCollection<CSharpCompilation>> CompileAsync()
    {
        var output = new Dictionary<ProjectId, CSharpCompilation>();

        foreach (var project in projects)
        {
            await CompileTransitivelyAsync(msbuildWorkspace, project.Id, output);
        }

        return output.Values;
    }

    private async Task CompileTransitivelyAsync(
        Workspace workspace,
        ProjectId id,
        Dictionary<ProjectId, CSharpCompilation> output)
    {
        if (output.ContainsKey(id))
        {
            return;
        }

        var project = workspace.CurrentSolution.GetProject(id);

        if (project is not null)
        {
            var compilation = await project
                .WithCompilationOptions(options)
                .AddUniqueMetadataReferences(ContractsCompiler.DefaultAssemblies)
                .GetCompilationAsync();

            if (compilation is CSharpCompilation cs)
            {
                output.Add(id, cs);

                foreach (var dependency in project.AllProjectReferences)
                {
                    await CompileTransitivelyAsync(workspace, dependency.ProjectId, output);
                }
            }
            else
            {
                throw new InvalidProjectException($"Cannot compile project {id}. The project does not support compilation.");
            }
        }
        else
        {
            throw new InvalidProjectException($"Cannot compile project - the project {id} cannot be located.");
        }
    }

    public void Dispose() => msbuildWorkspace.Dispose();
}

public static class ProjectExtensions
{
    public static Project AddUniqueMetadataReferences(
        this Project project,
        IEnumerable<MetadataReference> metadataReferences)
    {
        var existingMetadataReferences = project.MetadataReferences
            .Select(mr => Path.GetFileName(mr.Display))
            .ToHashSet();

        var newMetadataReferences = metadataReferences
            .Where(mr => !existingMetadataReferences.Contains(Path.GetFileName(mr.Display)))
            .ToList();

        return project.AddMetadataReferences(newMetadataReferences);
    }
}
