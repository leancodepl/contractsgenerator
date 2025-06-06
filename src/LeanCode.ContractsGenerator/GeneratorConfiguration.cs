namespace LeanCode.ContractsGenerator;

public sealed record class GeneratorConfiguration(bool AllowDateTime)
{
    public static GeneratorConfiguration Default { get; } = new(false);

    public GeneratorConfiguration(IGenerationOptions options)
        : this(AllowDateTime: options.AllowDateTime) { }
}
