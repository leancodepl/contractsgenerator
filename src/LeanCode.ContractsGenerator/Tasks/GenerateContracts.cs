using Google.Protobuf;
using LeanCode.ContractsGenerator.Compilation;
using LeanCode.ContractsGenerator.Generation;
using Microsoft.Build.Framework;

namespace LeanCode.ContractsGenerator.Tasks;

public class GenerateContracts : Microsoft.Build.Utilities.Task
{
    [Required]
    public string[] Contracts { get; set; } = Array.Empty<string>();

    [Required]
    public string OutputDir { get; set; } = string.Empty;

    [Output]
    public string CompiledContracts { get; set; } = string.Empty;

    public override bool Execute()
    {
        // TODO: fix, because this will always be "LeanCode.ContractsGenerator" when executed from our .targets file
        var projectName = Path.GetFileNameWithoutExtension(BuildEngine.ProjectFileOfTaskNode);

        try
        {
            var contracts = ContractsCompiler.CompileFiles(projectName, Contracts);
            var generated = new Generation.ContractsGenerator(contracts).Generate();
            CompiledContracts = Path.Combine(OutputDir, "LeanCode.Contracts.pb");
            using var outputStream = File.OpenWrite(CompiledContracts);
            generated.WriteTo(outputStream);
            Log.LogMessage(MessageImportance.Low, "Compiled contracts saved as {0}.", CompiledContracts);
            return true;
        }
        catch (CompilationFailedException e)
        {
            Log.LogError("Failed to compile contracts for project {0}: {1}\n{2}", projectName, e.Message, e.StackTrace);

            foreach (var diag in e.Diagnostics)
            {
                Log.LogError("{0}", diag.ToString());
            }
        }
        catch (GenerationFailedException e)
        {
            Log.LogError("Failed to generate contracts for project {0}: {1}\n{2}", projectName, e.Message, e.StackTrace);
        }
        catch (AnalyzeFailedException ex)
        {
            Log.LogError("Failed to generate contracts for project {0}, analyzers reported following errors:", projectName);

            foreach (var d in ex.Errors)
            {
                Log.LogError("[{0}] {1}\n    at {2}", d.Code, d.Message, d.Context.Path);
            }
        }
        catch (Exception e)
        {
            Log.LogError("Failed to generate contracts for project {0}: {1}\n{2}", projectName, e.Message, e.StackTrace);
        }

        return false;
    }
}
