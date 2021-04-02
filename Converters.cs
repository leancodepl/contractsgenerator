using System;
using System.Text.Json;

namespace LeanCode.ContractsGeneratorV2
{
    public sealed class ValueRefConverter : WriteOnlyJsonConverter<ValueRef>
    {
        public override void Write(Utf8JsonWriter writer, ValueRef value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case ValueRef.Null _:
                    writer.WriteNull("null");
                    break;
                case ValueRef.Number n:
                    writer.WriteNumber("number", n.Value);
                    break;
                case ValueRef.FloatingPoint f:
                    writer.WriteNumber("floating", f.Value);
                    break;
                case ValueRef.String s:
                    writer.WriteString("string", s.Value);
                    break;
                case ValueRef.Boolean b:
                    writer.WriteBoolean("boolean", b.Value);
                    break;

                default: throw new NotSupportedException($"ValueRef {value} is not supported.");
            }

            writer.WriteEndObject();
        }
    }

    public sealed class AttributeArgumentConverter : WriteOnlyJsonConverter<AttributeArgument>
    {
        public override void Write(Utf8JsonWriter writer, AttributeArgument value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case AttributeArgument.Positional p:
                    writer.WriteStartObject();
                    writer.WriteNumber("position", p.Position);
                    writer.WritePropertyName("value");
                    JsonSerializer.Serialize(writer, p.Value, options);
                    writer.WriteEndObject();
                    break;

                case AttributeArgument.Named p:
                    writer.WriteStartObject();
                    writer.WriteString("name", p.Name);
                    writer.WritePropertyName("value");
                    JsonSerializer.Serialize(writer, p.Value, options);
                    writer.WriteEndObject();
                    break;

                default: throw new NotSupportedException($"AttributeArgument {value} is not supported.");
            };
        }
    }

    public class GenericArgumentConverter : WriteOnlyJsonConverter<GenericArgument>
    {
        public override void Write(Utf8JsonWriter writer, GenericArgument value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            switch (value)
            {
                case GenericArgument.Param p:
                    writer.WriteString("param", p.Name);
                    break;

                case GenericArgument.Type t:
                    writer.WritePropertyName("type");
                    JsonSerializer.Serialize(writer, t.Ref, options);
                    break;

                default: throw new NotSupportedException($"GenericArgument {value} is not supported.");
            }
            writer.WriteEndObject();
        }
    }

    public sealed class TypeRefConverter : WriteOnlyJsonConverter<TypeRef>
    {
        public override void Write(Utf8JsonWriter writer, TypeRef value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case TypeRef.Generic g:
                    writer.WriteString("generic", g.Name);
                    break;

                case TypeRef.Internal i:
                    writer.WriteString("internal", i.Name);
                    if (i.GenericArguments.Count > 0)
                    {
                        writer.WritePropertyName("genericArguments");
                        JsonSerializer.Serialize(writer, i.GenericArguments, options);
                    }
                    break;

                case TypeRef.Known k:
                    writer.WritePropertyName("knownType");
                    JsonSerializer.Serialize(writer, k.Type, options);
                    if (k.GenericArguments.Count > 0)
                    {
                        writer.WritePropertyName("genericArguments");
                        JsonSerializer.Serialize(writer, k.GenericArguments, options);
                    }
                    break;

                default: throw new NotSupportedException($"TypeRef {value} is not supported.");
            }

            writer.WriteEndObject();
        }
    }

    public sealed class ErrorCodeConverter : WriteOnlyJsonConverter<ErrorCode>
    {
        public override void Write(Utf8JsonWriter writer, ErrorCode value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case ErrorCode.Single s:
                    writer.WriteString("single", s.Name);
                    writer.WriteNumber("code", s.Code);
                    break;

                case ErrorCode.Group g:
                    writer.WriteString("group", g.Name);
                    writer.WriteString("groupId", g.GroupId);
                    writer.WritePropertyName("innerCodes");
                    JsonSerializer.Serialize(writer, g.InnerCodes, options);
                    break;

                default: throw new NotSupportedException($"ErrorCode {value} is not supported.");
            }

            writer.WriteEndObject();
        }
    }

    public sealed class EnumStatementConverter : WriteOnlyJsonConverter<Statement.EnumStatement>
    {
        public override void Write(Utf8JsonWriter writer, Statement.EnumStatement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("name", value.Name);

            writer.WritePropertyName("members");
            writer.WriteStartObject();
            foreach (var e in value.Members)
            {
                writer.WriteNumber(e.Name, e.Value);
            }

            writer.WriteEndObject();

            writer.WriteEndObject();
        }
    }

    public sealed class StatementConverter : WriteOnlyJsonConverter<Statement>
    {
        public override void Write(Utf8JsonWriter writer, Statement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            switch (value)
            {
                case Statement.EnumStatement e:
                    writer.WritePropertyName("enum");
                    JsonSerializer.Serialize(writer, e, options);
                    break;

                case Statement.TypeStatement.DTOStatement d:
                    writer.WritePropertyName("dto");
                    JsonSerializer.Serialize(writer, d, options);
                    break;

                case Statement.TypeStatement.QueryStatement q:
                    writer.WritePropertyName("query");
                    JsonSerializer.Serialize(writer, q, options);
                    break;

                case Statement.TypeStatement.CommandStatement c:
                    writer.WritePropertyName("command");
                    JsonSerializer.Serialize(writer, c, options);
                    break;

                default: throw new NotSupportedException($"Statement {value} is not supported.");
            }
            writer.WriteEndObject();
        }
    }
}
