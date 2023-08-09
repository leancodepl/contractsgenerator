namespace LeanCode.ContractsGenerator.Generation;

public static class ObjectExtensions
{
    public static ValueRef ToValueRef(this object? val)
    {
        return val switch
        {
            null => new() { Null = new() },
            byte v => new ValueRef { Number = new() { Value = v } },
            sbyte v => new ValueRef { Number = new() { Value = v } },
            int v => new ValueRef { Number = new() { Value = v } },
            short v => new ValueRef { Number = new() { Value = v } },
            long v => new ValueRef { Number = new() { Value = v } },
            ushort v => new ValueRef { Number = new() { Value = v } },
            uint v => new ValueRef { Number = new() { Value = v } },
            ulong v => new ValueRef { Number = new() { Value = (long)v } },
            float v => new ValueRef { FloatingPoint = new() { Value = v } },
            double v => new ValueRef { FloatingPoint = new() { Value = v } },
            string v => new ValueRef { String = new() { Value = v } },
            bool v => new ValueRef { Bool = new() { Value = v } },
            _
                => throw new NotSupportedException(
                    $"Cannot generate contracts for constant of type {val.GetType()}."
                ),
        };
    }
}
