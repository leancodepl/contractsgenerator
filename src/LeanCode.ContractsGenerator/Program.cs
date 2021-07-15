using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Google.Protobuf;

namespace LeanCode.ContractsGenerator
{
    public interface IOptions
    {
        public const string StdoutMarker = "-";

        [Option('o', "output", Required = true, MetaValue = "FILE", HelpText = "The output file path. Use `-` to write the resulting Protobuf file to `stdout`.")]
        public string OutputFile { get; set; }
    }

    [Verb("project", HelpText = "Generate contracts from C# project.")]
    public class ProjectOptions : IOptions
    {
        public string OutputFile { get; set; }

        [Option('p', "project", Required = true, MetaValue = "FILE", HelpText = "The project file with contracts.")]
        public string ProjectFile { get; set; }
    }

    [Verb("file", HelpText = "Generate contracts from a single file.")]
    public class FileOptions : IOptions
    {
        public string OutputFile { get; set; }

        [Option('i', "input", Required = true, MetaValue = "FILE", HelpText = "Input file.")]
        public string InputFile { get; set; }
    }

    [Verb("path", HelpText = "Generate contracts based on globbed path.")]
    public class PathOptions : IOptions
    {
        public string OutputFile { get; set; }

        [Option('p', "path", Required = true, MetaValue = "FILE", HelpText = "Input path (will glob all .cs files).")]
        public string InputPath { get; set; }
    }

    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<ProjectOptions, FileOptions, PathOptions>(args)
                .MapResult(
                    (ProjectOptions p) => HandleProjectAsync(p),
                    (FileOptions f) => HandleFileAsync(f),
                    (PathOptions p) => HandlePathAsync(p),
                    err => Task.FromResult(1));
        }

        private static async Task<int> HandleProjectAsync(ProjectOptions p)
        {
            var contracts = await ContractsCompiler.CompileProjectsAsync(new[] { p.ProjectFile });
            return await WriteAsync(contracts, p.OutputFile);
        }

        private static async Task<int> HandleFileAsync(FileOptions f)
        {
            var contracts = await ContractsCompiler.CompileFileAsync(f.InputFile);
            return await WriteAsync(contracts, f.OutputFile);
        }

        private static async Task<int> HandlePathAsync(PathOptions p)
        {
            var contracts = await ContractsCompiler.CompilePathAsync(p.InputPath);
            return await WriteAsync(contracts, p.OutputFile);
        }

        private static async Task<int> WriteAsync(CompiledContracts contracts, string output)
        {
            var generated = new ContractsGenerator(contracts).Generate();
            if (output == IOptions.StdoutMarker)
            {
                await WriteToStdoutAsync(generated);
            }
            else
            {
                await WriteToFileAsync(generated, output);
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
    }
}
