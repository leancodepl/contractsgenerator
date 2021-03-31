using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeanCode.ContractsGeneratorV2
{
    public abstract class WriteOnlyJsonConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
