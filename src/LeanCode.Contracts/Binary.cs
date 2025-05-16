using System.Diagnostics.CodeAnalysis;

namespace LeanCode.Contracts;

[System.Text.Json.Serialization.JsonConverter(typeof(Converters.BinaryJsonConverter))]
public readonly record struct Binary : IEquatable<Binary>
{
    private readonly byte[]? data;

    public readonly byte[] Data => data ?? [];

    public Binary(byte[]? data)
    {
        this.data = data;
    }

    public bool Equals(Binary other) => MemoryExtensions.SequenceEqual(Data, other.Data);

    public override int GetHashCode() =>
        data switch
        {
            { LongLength: > 4 } d => HashCode.Combine(
                BitConverter.ToInt32(d.AsSpan(0..4)),
                BitConverter.ToInt32(d.AsSpan(^4..^0))
            ),
            { LongLength: 4 } d => BitConverter.ToInt32(d),
            { LongLength: 2 or 3 } d => BitConverter.ToInt16(d),
            { LongLength: 1 } d => d[0],
            _ => 0,
        };

    [return: NotNullIfNotNull(nameof(binary))]
    public static implicit operator byte[]?(Binary? binary) => binary?.Data;
}
