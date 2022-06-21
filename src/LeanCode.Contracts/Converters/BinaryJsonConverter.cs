using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeanCode.Contracts.Converters;

internal class BinaryJsonConverter : JsonConverter<Binary>
{
    public override bool HandleNull => false;

    public override Binary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.GetBytesFromBase64());

    public override void Write(Utf8JsonWriter writer, Binary value, JsonSerializerOptions options) =>
        writer.WriteBase64StringValue(value.Data);
}
