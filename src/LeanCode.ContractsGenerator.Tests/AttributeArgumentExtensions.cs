using LeanCode.ContractsGenerator.Generation;

namespace LeanCode.ContractsGenerator.Tests;

public static class AttributeArgumentExtensions
{
    public static AttributeArgument Named(string name, object? value)
    {
        return new()
        {
            Named = new() { Name = name, Value = value.ToValueRef(), },
        };
    }

    public static AttributeArgument Positional(int pos, object? value)
    {
        return new()
        {
            Positional = new() { Position = pos, Value = value.ToValueRef(), },
        };
    }
}
