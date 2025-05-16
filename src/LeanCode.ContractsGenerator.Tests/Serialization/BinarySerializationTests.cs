using System.Text.Json;
using LeanCode.Contracts;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.Serialization;

public class BinarySerializationTests
{
    private sealed record class BinaryDTO(Binary? Null, Binary? NullableValue, Binary Value);

    private static readonly byte[] Hello = [.. "hello"u8];

    private static readonly BinaryDTO DTO = new(null, Hello?.AsBinary(), Hello!.AsBinary());

    private static readonly string Json = $$"""
        {
          "{{nameof(BinaryDTO.Null)}}": null,
          "{{nameof(BinaryDTO.NullableValue)}}": "aGVsbG8=",
          "{{nameof(BinaryDTO.Value)}}": "aGVsbG8="
        }
        """;

    [Fact]
    public void Binary_is_serialized_as_base64_string()
    {
        var serialized = JsonSerializer.Serialize(DTO, new JsonSerializerOptions { WriteIndented = true });

        Assert.Equal(Json, serialized);
    }

    [Fact]
    public void Binary_can_be_deserialized_from_base64_string()
    {
        var deserialized = JsonSerializer.Deserialize<BinaryDTO>(Json);

        Assert.Equal(DTO, deserialized);
    }
}
