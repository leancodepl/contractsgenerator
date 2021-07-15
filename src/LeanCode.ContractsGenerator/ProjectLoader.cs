using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LeanCode.ContractsGenerator
{
    public class ProjectLoader
    {
        private static readonly IReadOnlySet<string> AllowedPackaged = new HashSet<string>
        {
            "LeanCode.CQRS",
            "LeanCode.Time",
        };

        private readonly CSharpCompilationOptions options;

        private readonly AnalyzerManager manager;
        private readonly List<IProjectAnalyzer> projects;

        public ProjectLoader(CSharpCompilationOptions options)
        {
            this.options = options;

            manager = new AnalyzerManager(null);
            projects = new List<IProjectAnalyzer>();
        }

        public void LoadProjects(IEnumerable<string> projectPaths)
        {
            var projs = projectPaths.Select(manager.GetProject);
            projects.AddRange(projs);
        }

        public void VerifyAll()
        {
            foreach (var p in projects)
            {
                VerifyProject(p);
            }
        }

        public async Task<IReadOnlyCollection<CSharpCompilation>> CompileAsync()
        {
            var output = new Dictionary<ProjectId, CSharpCompilation>();

            var workspace = GetWorkspace();
            foreach (var p in projects)
            {
                var id = ProjectId.CreateFromSerialized(p.ProjectGuid);
                await CompileTransitivelyAsync(workspace, id, output);
            }

            return output.Values;
        }

        private static void VerifyProject(IProjectAnalyzer project)
        {
            foreach (var package in project.ProjectFile.PackageReferences)
            {
                if (!AllowedPackaged.Contains(package.Name))
                {
                    throw new InvalidProjectException($"The project references package {package.Name} that is not allowed.");
                }
            }
        }

        private async Task CompileTransitivelyAsync(
            AdhocWorkspace workspace,
            ProjectId id,
            Dictionary<ProjectId, CSharpCompilation> output)
        {
            if (output.ContainsKey(id))
            {
                return;
            }

            var proj = workspace.CurrentSolution.GetProject(id);
            if (proj is null)
            {
                throw new InvalidProjectException($"Cannot compile project - the project {id} cannot be located.");
            }

            var compilation = await proj
                .WithCompilationOptions(options)
                .GetCompilationAsync();
            output.Add(id, (CSharpCompilation)compilation);

            foreach (var dependency in proj.AllProjectReferences)
            {
                await CompileTransitivelyAsync(workspace, dependency.ProjectId, output);
            }
        }

        public AdhocWorkspace GetWorkspace()
        {
            var list = manager.Projects.Values.AsParallel()
                .Select(x => x.Build().FirstOrDefault())
                .Where(x => x != null)
                .ToList();
            var adhocWorkspace = new AdhocWorkspace();
            var solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Default, manager.SolutionFilePath);
            adhocWorkspace.AddSolution(solutionInfo);

            foreach (var item in list)
            {
                var projectId = ProjectId.CreateFromSerialized(item.ProjectGuid);
                if (!adhocWorkspace.CurrentSolution.ContainsProject(projectId))
                {
                    item.AddToWorkspace(adhocWorkspace, true);
                }
            }

            return adhocWorkspace;
        }
    }
}
