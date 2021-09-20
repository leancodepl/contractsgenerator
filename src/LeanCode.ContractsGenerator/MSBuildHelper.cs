// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace LeanCode.ContractsGenerator;

public static class MSBuildHelper
{
    static MSBuildHelper()
    {
        // QueryVisualStudioInstances returns Visual Studio installations on .NET Framework, and .NET Core SDK
        // installations on .NET Core. We use the one with the most recent version.
        var msBuildInstance = MSBuildLocator.QueryVisualStudioInstances().OrderByDescending(x => x.Version).First();

        // Since we do not inherit msbuild.deps.json when referencing the SDK copy
        // of MSBuild and because the SDK no longer ships with version matched assemblies, we
        // register an assembly loader that will load assemblies from the msbuild path with
        // equal or higher version numbers than requested.
        LooseVersionAssemblyLoader.Register(msBuildInstance.MSBuildPath);

        MSBuildLocator.RegisterInstance(msBuildInstance);
    }

    public static MSBuildWorkspace CreateWorkspace()
    {
        return MSBuildWorkspace
            .Create(new Dictionary<string, string>
            {
                // This property ensures that XAML files will be compiled in the current AppDomain
                // rather than a separate one. Any tasks isolated in AppDomains or tasks that create
                // AppDomains will likely not work due to https://github.com/Microsoft/MSBuildLocator/issues/16.
                ["AlwaysCompileMarkupFilesInSeparateDomain"] = bool.FalseString,

                // Use the preview language version to force the full set of available analyzers to run on the project.
                ["LangVersion"] = "preview",

                ["TargetFrameworks"] = "net6.0",
                ["TargetFramework"] = "net6.0",
            });
    }
}
