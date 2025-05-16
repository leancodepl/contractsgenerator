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

    private static readonly object Guard = new();
    private static readonly IReadOnlyCollection<string> Extensions = ["ni.dll", "ni.exe", "dll", "exe"];
    private static readonly IReadOnlyCollection<string> NoSubfolders = [string.Empty];

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
        // If no culture is specified, attempt to load directly from
        // the known dependency paths.
        IEnumerable<string> cultureSubfolders = NoSubfolders;

        if (!string.IsNullOrEmpty(assemblyName.CultureName))
        {
            // Search for satellite assemblies in culture subdirectories
            // of the assembly search directories, but fall back to the
            // bare search directory if that fails.
            cultureSubfolders = cultureSubfolders.Prepend(assemblyName.CultureName);
        }

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
