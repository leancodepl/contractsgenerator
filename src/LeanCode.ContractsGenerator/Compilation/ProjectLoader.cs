using System.Xml;
using LeanCode.ContractsGenerator.Compilation.MSBuild;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

using MSB = Microsoft.Build;

namespace LeanCode.ContractsGenerator.Compilation;

public class ProjectLoader : IDisposable
{
    private readonly CSharpCompilationOptions options;

    private readonly MSBuildWorkspace msbuildWorkspace = MSBuildHelper.CreateWorkspace();
    private readonly List<Project> projects = new();

    public ProjectLoader(CSharpCompilationOptions options)
    {
        this.options = options;

        msbuildWorkspace.WorkspaceFailed += (_, f) => Console.WriteLine(f.Diagnostic);
    }

    public async Task LoadProjectsAsync(IEnumerable<string> projectPaths)
    {
        foreach (var projectPath in projectPaths)
        {
            var projectFullPath = ResolveCanonicalPath(projectPath);

            if (msbuildWorkspace.CurrentSolution.Projects
                .Where(p => p.FilePath is not null)
                .Any(p => ResolveCanonicalPath(p.FilePath) == projectFullPath))
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
        // await RestoreProjects();
        var output = new Dictionary<ProjectId, CSharpCompilation>();

        foreach (var project in projects)
        {
            await CompileTransitivelyAsync(msbuildWorkspace, project.Id, output);
        }

        return output.Values;
    }

    private async Task RestoreProjects()
    {
        var batchBuildProjectCollection = new MSB.Evaluation.ProjectCollection(RestoreProperties);
        var buildParams = new MSB.Execution.BuildParameters(batchBuildProjectCollection);

        foreach (var p in projects)
        {
            using var ms = new MemoryStream();
            await using (var stream = File.OpenRead(p.FilePath!))
            {
                await stream.CopyToAsync(ms);
                ms.Position = 0;
            }

            using var xmlReader = XmlReader.Create(ms, s_xmlReaderSettings);
            var xml = MSB.Construction.ProjectRootElement.Create(xmlReader, batchBuildProjectCollection);

            // When constructing a project from an XmlReader, MSBuild cannot determine the project file path.  Setting the
            // path explicitly is necessary so that the reserved properties like $(MSBuildProjectDirectory) will work.
            xml.FullPath = p.FilePath!;

            var project = new MSB.Evaluation.Project(xml, globalProperties: null, toolsVersion: null, batchBuildProjectCollection);
        }

        MSB.Execution.BuildManager.DefaultBuildManager.BeginBuild(buildParams);
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

    private static readonly Dictionary<string, string> RestoreProperties = new()
    {
        { "DesignTimeBuild", bool.TrueString },
        { "NonExistentFile", "__NonExistentSubDir__\\__NonExistentFile__" },
        { "BuildProjectReferences", bool.FalseString },
        { "BuildingProject", bool.FalseString },
        { "ProvideCommandLineArgs", bool.TrueString },
        { "SkipCompilerExecution", bool.TrueString },
        { "ContinueOnError", "ErrorAndContinue" },
        { "ShouldUnsetParentConfigurationAndPlatform", bool.FalseString },
        { "AlwaysCompileMarkupFilesInSeparateDomain", bool.FalseString },
    };
    private static readonly XmlReaderSettings s_xmlReaderSettings = new()
    {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null
    };
}
