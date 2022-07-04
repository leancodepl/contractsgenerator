namespace LeanCode.Contracts;

[System.Text.Json.Serialization.JsonConverter(typeof(Converters.BinaryJsonConverter))]
public record struct Binary : IEquatable<Binary>
{
    private readonly byte[]? data;

    public byte[] Data => data ?? Array.Empty<byte>();

    public Binary(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        this.data = data;
    }

    public bool Equals(Binary other)
    {
        var otherData = other.Data;
        return Data.Length == otherData.Length && Data.SequenceEqual(otherData);
    }

    public override int GetHashCode() => Data.Length;

#if NET7_0_OR_GREATER
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(binary))]
#else
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("binary")]
#endif
    public static implicit operator byte[]?(Binary? binary) => binary?.Data;
}
