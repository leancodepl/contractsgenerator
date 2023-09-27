using CommandLine;
using Google.Protobuf;
using LeanCode.ContractsGenerator.Compilation;
using LeanCode.ContractsGenerator.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.FileSystemGlobbing;

namespace LeanCode.ContractsGenerator;

public interface IOptions
{
    public const string StdoutMarker = "-";

    [Option(
        'o',
        "output",
        MetaValue = "FILE",
        HelpText = "The output file path. Use `-` to write the resulting Protobuf file to `stdout`.",
        Group = "output"
    )]
    public string OutputFile { get; set; }

    [Option(
        "check-only",
        HelpText = "Check the contracts only. Do not generate output.",
        Group = "output"
    )]
    public bool CheckOnly { get; set; }
}

[Verb("project", HelpText = "Generate contracts from C# project.")]
public class ProjectOptions : IOptions
{
    public string OutputFile { get; set; } = string.Empty;
    public bool CheckOnly { get; set; }

    [Option(
        "exclude-external-contracts-from-output",
        Required = false,
        HelpText = "Do not include contracts from referenced assemblies in the output. Analyzers will still run on the merged result."
    )]
    public bool ExcludeExternalContractsFromOutput { get; set; } = false;

    [Option(
        'p',
        "project",
        Required = true,
        MetaValue = "FILE",
        HelpText = "The project file with contracts. To pass multiple projects, separate the values with space."
    )]
    public IEnumerable<string> ProjectFiles { get; set; } = Array.Empty<string>();
}

[Verb("file", HelpText = "Generate contracts from a single file.")]
public class FileOptions : IOptions
{
    public string OutputFile { get; set; } = string.Empty;
    public bool CheckOnly { get; set; }

    [Option('i', "input", Required = true, MetaValue = "FILE", HelpText = "Input file.")]
    public string InputFile { get; set; } = string.Empty;
}

[Verb("path", HelpText = "Generate contracts based on globbed path.")]
public class PathOptions : IOptions
{
    public string OutputFile { get; set; } = string.Empty;
    public bool CheckOnly { get; set; }

    [Option(
        'i',
        "include",
        Required = true,
        MetaValue = "PATTERN",
        HelpText = "Include files from glob pattern. To pass multiple patterns, separate them with space."
    )]
    public IEnumerable<string> Include { get; set; } = Array.Empty<string>();

    [Option(
        'e',
        "exclude",
        Required = false,
        MetaValue = "PATTERN",
        HelpText = "Exclude files from glob pattern. Has higher precedence than includes. To pass multiple patterns, separate them with space."
    )]
    public IEnumerable<string> Exclude { get; set; } = Array.Empty<string>();

    [Option(
        'd',
        "directory",
        MetaValue = "PATH",
        HelpText = "The base directory used for globbing. Uses current directory if not specified."
    )]
    public string? BaseDirectory { get; set; }
}

internal class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "?",
        "CA1031",
        Justification = "Exception boundary."
    )]
    private static async Task<int> Main(string[] args)
    {
        try
        {
            return await Parser.Default
                .ParseArguments<ProjectOptions, FileOptions, PathOptions>(args)
                .MapResult(
                    (ProjectOptions p) => HandleProjectAsync(p),
                    (FileOptions f) => HandleFileAsync(f),
                    (PathOptions p) => HandlePathAsync(p),
                    err => Task.FromResult(1)
                );
        }
        catch (InvalidProjectException ex)
        {
            await Console.Error.WriteLineAsync($"Cannot load one of the projects: {ex.Message}");
            await Console.Error.WriteLineAsync("At");
            await Console.Error.WriteLineAsync(ex.StackTrace);
            return 2;
        }
        catch (CompilationFailedException ex)
        {
            await Console.Error.WriteLineAsync(
                $"Cannot compile contracts. There were errors during project compilation: {ex.Message}"
            );

            return 3;
        }
        catch (GenerationFailedException ex)
        {
            await Console.Error.WriteLineAsync($"Cannot generate contracts: {ex.Message}");
            await Console.Error.WriteLineAsync("At");
            await Console.Error.WriteLineAsync(ex.StackTrace);
            return 4;
        }
        catch (AnalyzeFailedException ex)
        {
            await Console.Error.WriteLineAsync(
                $"Cannot generate contracts. The analyze phase failed with following errors:"
            );
            foreach (var d in ex.Errors)
            {
                await Console.Error.WriteLineAsync($"[{d.Code}] {d.Message}");
                await Console.Error.WriteLineAsync($"    at {d.Context.Path}");
            }

            return 5;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(
                $"Cannot compile project or generate contracts: {ex.Message}"
            );
            await Console.Error.WriteLineAsync("At");
            await Console.Error.WriteLineAsync(ex.StackTrace);
            return 6;
        }
    }

    private static async Task<int> HandleProjectAsync(ProjectOptions p)
    {
        var (compiled, external) = await ContractsCompiler.CompileProjectsAsync(p.ProjectFiles);
        return await WriteAsync(
            p,
            compiled,
            external,
            p.ExcludeExternalContractsFromOutput,
            p.OutputFile
        );
    }

    private static async Task<int> HandleFileAsync(FileOptions f)
    {
        var contracts = await ContractsCompiler.CompileFileAsync(f.InputFile);
        return await WriteAsync(f, contracts, f.OutputFile);
    }

    private static async Task<int> HandlePathAsync(PathOptions p)
    {
        var matcher = new Matcher();
        matcher.AddIncludePatterns(p.Include);
        matcher.AddExcludePatterns(p.Exclude);
        var directory = new DirectoryInfo(p.BaseDirectory ?? Directory.GetCurrentDirectory());
        var contracts = await ContractsCompiler.CompileGlobAsync(matcher, directory);
        return await WriteAsync(p, contracts, p.OutputFile);
    }

    private static async Task<int> WriteAsync(
        IOptions opts,
        CompiledContracts contracts,
        string output
    )
    {
        var generated = new Generation.ContractsGenerator(contracts).Generate();
        if (!opts.CheckOnly)
        {
            if (output == IOptions.StdoutMarker)
            {
                await WriteToStdoutAsync(generated);
            }
            else
            {
                await WriteToFileAsync(generated, output);
            }
        }

        return 0;
    }

    private static async Task<int> WriteAsync(
        IOptions opts,
        CompiledContracts contracts,
        List<Export> externalContracts,
        bool excludeExternalContractsFromOutput,
        string output
    )
    {
        var generated = new Generation.ContractsGenerator(contracts).Generate(
            externalContracts,
            excludeExternalContractsFromOutput
        );

        if (!opts.CheckOnly)
        {
            if (output == IOptions.StdoutMarker)
            {
                await WriteToStdoutAsync(generated);
            }
            else
            {
                await WriteToFileAsync(generated, output);
            }
        }

        return 0;
    }

    private static async Task WriteToFileAsync(Export generated, string filepath)
    {
        await using var outputStream = File.OpenWrite(filepath);
        using var codedOutput = new CodedOutputStream(outputStream, true);
        generated.WriteTo(codedOutput);
    }

    private static async Task WriteToStdoutAsync(Export generated)
    {
        await using var outputStream = System.Console.OpenStandardOutput();
        using var codedOutput = new CodedOutputStream(outputStream, true);
        generated.WriteTo(codedOutput);
    }

    private static string FormatLocation(Location location)
    {
        var lineSpan = location.GetMappedLineSpan();
        if (lineSpan.Path is not null)
        {
            return lineSpan.Path
                + "@"
                + (lineSpan.StartLinePosition.Line + 1)
                + ":"
                + (lineSpan.StartLinePosition.Character + 1);
        }
        else if (location.IsInSource)
        {
            return location.Kind
                + "("
                + location.SourceTree?.FilePath
                + location.SourceSpan.ToString()
                + ")";
        }
        else if (location.IsInMetadata && location.MetadataModule is not null)
        {
            return location.Kind + "(" + location.MetadataModule.Name + ")";
        }
        else
        {
            return location.Kind.ToString();
        }
    }
}
