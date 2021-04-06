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
                case { Null: var n } when n is not null:
                    writer.WriteNull("null");
                    break;
                case { Number: var n } when n is not null:
                    writer.WriteNumber("number", n.Value);
                    break;
                case { FloatingPoint: var f } when f is not null:
                    writer.WriteNumber("floating", f.Value);
                    break;
                case { String: var s } when s is not null:
                    writer.WriteString("string", s.Value);
                    break;
                case { Bool: var b } when b is not null:
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
                case { Positional: var p } when p is not null:
                    writer.WriteStartObject();
                    writer.WriteNumber("position", p.Position);
                    writer.WritePropertyName("value");
                    JsonSerializer.Serialize(writer, p.Value, options);
                    writer.WriteEndObject();
                    break;

                case { Named: var p } when p is not null:
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
                case { Param: var p } when p is not null:
                    writer.WriteString("param", p.Name);
                    break;

                case { Type: var t } when t is not null:
                    writer.WritePropertyName("type");
                    JsonSerializer.Serialize(writer, t.Type_, options);
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
                case { Generic: var g } when g is not null:
                    writer.WriteString("generic", g.Name);
                    break;

                case { Internal: var i } when i is not null:
                    writer.WriteString("internal", i.Name);
                    if (i.Arguments.Count > 0)
                    {
                        writer.WritePropertyName("arguments");
                        JsonSerializer.Serialize(writer, i.Arguments, options);
                    }
                    break;

                case { Known: var k } when k is not null:
                    writer.WritePropertyName("knownType");
                    JsonSerializer.Serialize(writer, k.Type, options);
                    if (k.Arguments.Count > 0)
                    {
                        writer.WritePropertyName("arguments");
                        JsonSerializer.Serialize(writer, k.Arguments, options);
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
                case { Single: var s } when s is not null:
                    writer.WriteString("single", s.Name);
                    writer.WriteNumber("code", s.Code);
                    break;

                case { Group: var g } when g is not null:
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

    public sealed class EnumStatementConverter : WriteOnlyJsonConverter<Statement.Types.Enum>
    {
        public override void Write(Utf8JsonWriter writer, Statement.Types.Enum value, JsonSerializerOptions options)
        {
            writer.WritePropertyName("members");

            // TODO: name
            writer.WriteStartObject();
            foreach (var e in value.Members)
            {
                writer.WriteNumber(e.Name, e.Value);
            }

            writer.WriteEndObject();
        }
    }

    public sealed class StatementConverter : WriteOnlyJsonConverter<Statement>
    {
        public override void Write(Utf8JsonWriter writer, Statement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            // switch (value)
            // {
            //     case Statement.EnumStatement e:
            //         writer.WritePropertyName("enum");
            //         JsonSerializer.Serialize(writer, e, options);
            //         break;

            //     case Statement.TypeStatement.DTOStatement d:
            //         writer.WritePropertyName("dto");
            //         JsonSerializer.Serialize(writer, d, options);
            //         break;

            //     case Statement.TypeStatement.QueryStatement q:
            //         writer.WritePropertyName("query");
            //         JsonSerializer.Serialize(writer, q, options);
            //         break;

            //     case Statement.TypeStatement.CommandStatement c:
            //         writer.WritePropertyName("command");
            //         JsonSerializer.Serialize(writer, c, options);
            //         break;

            //     default: throw new NotSupportedException($"Statement {value} is not supported.");
            // }
            writer.WriteEndObject();
        }
    }
}
