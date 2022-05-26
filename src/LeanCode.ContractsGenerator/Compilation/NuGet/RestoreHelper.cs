using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using LeanCode.ContractsGenerator.Compilation.MSBuild;
using NuGet.Build;
using NuGet.Build.Tasks;
using NuGet.Common;
using NuGet.ProjectModel;

namespace LeanCode.ContractsGenerator.Compilation.NuGet;

public static class RestoreHelper
{
    private delegate DependencyGraphSpec GetDependencyGraphSpecDelegate(
        string entryProjectPath, IDictionary<string, string> globalProperties);

    private static readonly Assembly? NuGetBuildTasksConsoleAssembly;
    private static readonly Type? MSBuildStaticGraphRestoreType;
    private static readonly PropertyInfo? MSBuildLoggerPropertyInfo;
    private static readonly PropertyInfo? LoggingQueuePropertyInfo;
    private static readonly MethodInfo? GetDependencyGraphSpecMethodInfo;
    private static readonly bool Loaded = false;

    static RestoreHelper()
    {
        RuntimeHelpers.RunClassConstructor(typeof(MSBuildHelper).TypeHandle);

        // NuGet assemblies are shipped alongside MSBuild in .NET SDK, we should be able to find them now
        NuGetBuildTasksConsoleAssembly = Assembly.Load("NuGet.Build.Tasks.Console");

        if (NuGetBuildTasksConsoleAssembly is null)
        {
            Console.Error.WriteLine("Failed to load NuGet.Build.Tasks.Console assembly.");
            return;
        }

        MSBuildStaticGraphRestoreType = NuGetBuildTasksConsoleAssembly
            .GetType("NuGet.Build.Tasks.Console.MSBuildStaticGraphRestore");

        if (MSBuildStaticGraphRestoreType is null)
        {
            Console.Error.WriteLine("Failed to find MSBuildStaticGraphRestore type.");
            return;
        }

        MSBuildLoggerPropertyInfo = MSBuildStaticGraphRestoreType.GetProperty(
            "MSBuildLogger",
            BindingFlags.Instance | BindingFlags.NonPublic);

        LoggingQueuePropertyInfo = MSBuildStaticGraphRestoreType.GetProperty(
            "LoggingQueue",
            BindingFlags.Instance | BindingFlags.NonPublic);

        GetDependencyGraphSpecMethodInfo = MSBuildStaticGraphRestoreType.GetMethod(
            "GetDependencyGraphSpec",
            BindingFlags.Instance | BindingFlags.NonPublic,
            new[] { typeof(string), typeof(IDictionary<string, string>) });

        if (MSBuildLoggerPropertyInfo is null ||
            LoggingQueuePropertyInfo is null ||
            GetDependencyGraphSpecMethodInfo is null)
        {
            Console.Error.WriteLine("Failed to find required methods in MSBuildStaticGraphRestore type.");
        }
        else
        {
            Loaded = true;
        }
    }

    public static async Task<bool> RestoreProjectsAsync(
        IReadOnlyCollection<string> projectPaths,
        IDictionary<string, string>? globalProperties = null,
        CancellationToken cancellationToken = default)
    {
        if (!Loaded)
        {
            return false;
        }

        using var instance = CreateMSBuildStaticGraphRestore(out var getDependencyGraphSpec, out var msBuildLogger);

        var dependencyGraphSpec = DependencyGraphSpec
            .Union(projectPaths
                .Select(p => getDependencyGraphSpec(p, globalProperties ??= ImmutableDictionary<string, string>.Empty))
                .Where(dgs => dgs is not null));

        foreach (var project in dependencyGraphSpec.Projects)
        {
            if (BuildTasksUtility.DoesProjectSupportRestore(project))
            {
                dependencyGraphSpec.AddRestore(project.RestoreMetadata.ProjectUniqueName);
            }
        }

        return await BuildTasksUtility.RestoreAsync(
            dependencyGraphSpec,
            interactive: false,
            recursive: true,
            noCache: false,
            ignoreFailedSources: true,
            disableParallel: false,
            force: false,
            forceEvaluate: false,
            hideWarningsAndErrors: true,
            restorePC: false,
            cleanupAssetsForUnsupportedProjects: false,
            log: msBuildLogger,
            cancellationToken: cancellationToken);
    }

    public static async Task<bool> RestoreProjectsSerializedAsync(
        IEnumerable<string> projectPaths,
        IDictionary<string, string>? globalProperties = null,
        CancellationToken cancellationToken = default)
    {
        if (!Loaded)
        {
            return false;
        }

        using var instance = CreateMSBuildStaticGraphRestore(out var getDependencyGraphSpec, out var msBuildLogger);

        var succeeded = true;

        foreach (var project in projectPaths)
        {
            var dependencyGraphSpec = getDependencyGraphSpec(
                project, globalProperties ??= ImmutableDictionary<string, string>.Empty);

            succeeded &= await BuildTasksUtility.RestoreAsync(
                dependencyGraphSpec,
                interactive: false,
                recursive: true,
                noCache: false,
                ignoreFailedSources: true,
                disableParallel: false,
                force: false,
                forceEvaluate: false,
                hideWarningsAndErrors: true,
                restorePC: false,
                cleanupAssetsForUnsupportedProjects: false,
                log: msBuildLogger,
                cancellationToken: cancellationToken);
        }

        return succeeded;
    }

    private static IDisposable CreateMSBuildStaticGraphRestore(
        out GetDependencyGraphSpecDelegate getDependencyGraphSpec, out MSBuildLogger msBuildLogger)
    {
        System.Diagnostics.Debug.Assert(Loaded, "MSBuildStaticGraphRestore must be loaded at this point.");

        var msBuildStaticGraphRestore = (IDisposable)Activator
            .CreateInstance(MSBuildStaticGraphRestoreType!, new object[] { false })!;

        msBuildLogger = (MSBuildLogger)MSBuildLoggerPropertyInfo!.GetValue(msBuildStaticGraphRestore)!;

#if !DEBUG
        // we can't easily control MSBuild's loggers _and_ they interfere with `-o-` option
        // so let's just suppress all the logging on Release builds
        msBuildLogger.VerbosityLevel = LogLevel.Error;

        var loggingQueue = (Microsoft.Build.Framework.ILogger)LoggingQueuePropertyInfo!.GetValue(msBuildStaticGraphRestore)!;

        loggingQueue.Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Quiet;
#endif

        getDependencyGraphSpec = GetDependencyGraphSpecMethodInfo!
            .CreateDelegate<GetDependencyGraphSpecDelegate>(msBuildStaticGraphRestore);

        return msBuildStaticGraphRestore;
    }
}
