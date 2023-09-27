// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections.Immutable;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using static Microsoft.Build.Execution.BuildRequestDataFlags;

namespace LeanCode.ContractsGenerator.Compilation.MSBuild;

public static class MSBuildHelper
{
    private static readonly string[] RestoreTarget = new string[] { "Restore" };

    private static readonly ImmutableDictionary<string, string> GlobalProperties =
        ImmutableDictionary.CreateRange(
            new Dictionary<string, string>
            {
                // This property ensures that XAML files will be compiled in the current AppDomain
                // rather than a separate one. Any tasks isolated in AppDomains or tasks that create
                // AppDomains will likely not work due to https://github.com/Microsoft/MSBuildLocator/issues/16.
                ["AlwaysCompileMarkupFilesInSeparateDomain"] = bool.FalseString,
                // Use the preview language version to force the full set of available analyzers to run on the project.
                ["LangVersion"] = "preview",
            }
        );

    static MSBuildHelper()
    {
        // QueryVisualStudioInstances returns Visual Studio installations on .NET Framework, and .NET Core SDK
        // installations on .NET Core. We use the one with the most recent version.
        var msBuildInstance = MSBuildLocator
            .QueryVisualStudioInstances()
            .OrderByDescending(x => x.Version)
            .First();

        // Since we do not inherit msbuild.deps.json when referencing the SDK copy
        // of MSBuild and because the SDK no longer ships with version matched assemblies, we
        // register an assembly loader that will load assemblies from the msbuild path with
        // equal or higher version numbers than requested.
        LooseVersionAssemblyLoader.Register(msBuildInstance.MSBuildPath);

        if (MSBuildLocator.CanRegister)
        {
            MSBuildLocator.RegisterInstance(msBuildInstance);
        }
    }

    public static MSBuildWorkspace CreateWorkspace(ImmutableDictionary<string, string> properties)
    {
        var finalProps = properties.AddRange(GlobalProperties);
        return MSBuildWorkspace.Create(finalProps);
    }

    public static int RestoreProjects(IReadOnlyCollection<string> projectPaths)
    {
        if (projectPaths.Count == 0)
        {
            return 0;
        }

        using var projectCollection = new ProjectCollection(GlobalProperties);
        var projects = new List<Project>(projectPaths.Count);

        foreach (var p in projectPaths)
        {
            projects.Add(projectCollection.LoadProject(p));
        }

        // use a separate instance of BuildManager to avoid confusing Roslyn's MSBuildWorkspace
        using var buildManager = new BuildManager();

        buildManager.BeginBuild(
            new BuildParameters(projectCollection)
            {
                // together with explicit PackageReference prevents tests from breaking
                // by attempting to load multiple versions of NuGet.Frameworks into one process
                DisableInProcNode = true,
                // don't ask the user for anything
                Interactive = false,
            }
        );

        var failed = 0;

        try
        {
            foreach (var project in projects)
            {
                var projectInstance = project.CreateProjectInstance();

                if (!projectInstance.Targets.ContainsKey("_IsProjectRestoreSupported"))
                {
                    continue;
                }

                var buildRequestData = new BuildRequestData(
                    projectInstance,
                    RestoreTarget,
                    hostServices: null,
                    flags: ClearCachesAfterBuild
                        | SkipNonexistentTargets
                        | IgnoreMissingEmptyAndInvalidImports
                );

                buildManager
                    .PendBuildRequest(buildRequestData)
                    .ExecuteAsync(
                        bs =>
                        {
                            if (bs.BuildResult.OverallResult != BuildResultCode.Success)
                            {
                                Interlocked.Increment(ref failed);
                            }
                        },
                        context: null
                    );
            }
        }
        finally
        {
            buildManager.EndBuild();
        }

        return failed;
    }
}
