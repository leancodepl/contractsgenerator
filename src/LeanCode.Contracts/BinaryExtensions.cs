namespace LeanCode.Contracts;

public static class BinaryExtensions
{
    public static Binary AsBinary(this byte[] data) => new(data);
}
