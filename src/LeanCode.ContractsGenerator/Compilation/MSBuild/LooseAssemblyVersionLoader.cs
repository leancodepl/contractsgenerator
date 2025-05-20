// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Reflection;
using System.Runtime.Loader;

namespace LeanCode.ContractsGenerator.Compilation.MSBuild;

internal static class LooseVersionAssemblyLoader
{
    private static readonly Dictionary<string, Assembly> PathsToAssemblies = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Assembly> NamesToAssemblies = [];

#if NET9_0_OR_GREATER
    private static readonly Lock Guard = new();
#else
    private static readonly object Guard = new();
#endif
    private static readonly IReadOnlyCollection<string> Extensions = ["ni.dll", "ni.exe", "dll", "exe"];

    /// <summary>
    /// Register an assembly loader that will load assemblies with higher version than what was requested.
    /// </summary>
    public static void Register(string searchPath) =>
        AssemblyLoadContext.Default.Resolving += (AssemblyLoadContext context, AssemblyName assemblyName) =>
        {
            lock (Guard)
            {
                if (NamesToAssemblies.TryGetValue(assemblyName.FullName, out var assembly))
                {
                    return assembly;
                }

                return TryResolveAssemblyFromPaths_NoLock(context, assemblyName, searchPath);
            }
        };

    private static Assembly TryResolveAssemblyFromPaths_NoLock(
        AssemblyLoadContext context,
        AssemblyName assemblyName,
        string searchPath
    )
    {
        Span<string> cultureSubfolders = string.IsNullOrEmpty(assemblyName.CultureName)
            // If no culture is specified, attempt to load directly from
            // the known dependency paths.
            ? [string.Empty]
            // Search for satellite assemblies in culture subdirectories
            // of the assembly search directories, but fall back to the
            // bare search directory if that fails.
            : [assemblyName.CultureName, string.Empty];

        foreach (var cultureSubfolder in cultureSubfolders)
        {
            foreach (var extension in Extensions)
            {
                var candidatePath = Path.Combine(searchPath, cultureSubfolder, $"{assemblyName.Name}.{extension}");

                var isAssemblyLoaded = PathsToAssemblies.ContainsKey(candidatePath);
                if (isAssemblyLoaded || !File.Exists(candidatePath))
                {
                    continue;
                }

                var candidateAssemblyName = AssemblyLoadContext.GetAssemblyName(candidatePath);
                if (candidateAssemblyName.Version < assemblyName.Version)
                {
                    continue;
                }

                return LoadAndCache_NoLock(context, candidatePath);
            }
        }

        return null;
    }

    private static Assembly LoadAndCache_NoLock(AssemblyLoadContext context, string fullPath)
    {
        var assembly = context.LoadFromAssemblyPath(fullPath);
        var name = assembly.FullName;

        PathsToAssemblies[fullPath] = assembly;
        NamesToAssemblies[name] = assembly;

        return assembly;
    }
}
