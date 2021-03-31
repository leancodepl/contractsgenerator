using System;
using System.Text.Json;

namespace LeanCode.ContractsGeneratorV2
{
    internal static class Utf8JsonWriterExtensions
    {
        public static void WriteValue(this Utf8JsonWriter writer, string propertyName, ValueRef v, JsonSerializerOptions options)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            JsonSerializer.Serialize(writer, v.Type, options);

            switch (v.Type)
            {
                case ValueType.Null:
                    writer.WriteNull("value");
                    break;
                case ValueType.Number:
                    writer.WriteNumber("value", Convert.ToInt64(v.Value));
                    break;
                case ValueType.FloatingPointNumber:
                    writer.WriteNumber("value", Convert.ToDouble(v.Value));
                    break;
                case ValueType.String:
                    writer.WriteString("value", (string)v.Value);
                    break;
                case ValueType.Boolean:
                    writer.WriteBoolean("value", (bool)v.Value);
                    break;

                default: throw new NotSupportedException($"ValueRef type {v.Type} is not supported.");
            }

            writer.WriteEndObject();
        }
    }
}
