﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Locator;
using Microsoft.Build.Logging;
using Microsoft.CodeAnalysis.MSBuild;
using static Microsoft.Build.Execution.BuildRequestDataFlags;

namespace LeanCode.ContractsGenerator.Compilation.MSBuild;

public static class MSBuildHelper
{
    private static readonly string[] RestoreTarget = ["Restore"];
    private static readonly string LoggerVerbosity = Environment.GetEnvironmentVariable("LNCD_CG_MSB_LOG");
    private static readonly bool LogEnabled = LoggerVerbosity is { Length: > 0 };

    private static readonly ImmutableDictionary<string, string> GlobalProperties = ImmutableDictionary.CreateRange(
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
        try
        {
            // QueryVisualStudioInstances returns Visual Studio installations on .NET Framework,
            // and .NET Core SDK installations on .NET Core. We use the one with the most recent version.
            var msBuildInstances = MSBuildLocator
                .QueryVisualStudioInstances()
                .OrderByDescending(x => x.Version)
                .ToList();

            if (LogEnabled)
            {
                Console.Error.WriteLine($"Running on {RuntimeInformation.FrameworkDescription}");
                Console.Error.WriteLine("MSBuild instances:");

                if (msBuildInstances.Count > 0)
                {
                    foreach (var instance in msBuildInstances)
                    {
                        Console.Error.WriteLine($"  {instance.Version} @ {instance.MSBuildPath}");
                    }
                }
                else
                {
                    Console.Error.WriteLine($"  none found");
                }
            }

            var msBuildInstance = msBuildInstances[0];

            if (MSBuildLocator.CanRegister)
            {
                MSBuildLocator.RegisterInstance(msBuildInstance);

                if (LogEnabled)
                {
                    Console.Error.WriteLine(
                        $"MSBuild instance {msBuildInstance.Version} @ {msBuildInstance.MSBuildPath} registered."
                    );
                }
            }
            else if (LogEnabled)
            {
                Console.Error.WriteLine(
                    $"Could not register found MSBuild instance {msBuildInstance.Version} @ {msBuildInstance.MSBuildPath}."
                );
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            throw;
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
                Loggers = Enum.TryParse<LoggerVerbosity>(LoggerVerbosity, true, out var verbosity)
                    ? [new ConsoleLogger(verbosity)]
                    : [],
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
                    flags: ClearCachesAfterBuild | SkipNonexistentTargets | IgnoreMissingEmptyAndInvalidImports
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
