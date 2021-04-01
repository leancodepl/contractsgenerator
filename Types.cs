using System.Collections.Immutable;
using System.Diagnostics;

namespace LeanCode.ContractsGeneratorV2
{
    public record Export(string BasePath, ImmutableList<Statement> Statements, ImmutableList<ErrorCode.Group> KnownErrorGroups);

    public enum ValueType
    {
        Null,
        Number,
        FloatingPointNumber,
        String,
        Boolean,
    }

    public enum KnownType
    {
        Object,
        String,

        UInt8,
        Int8,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,

        Float,
        Double,
        Decimal,

        Guid,
        Uri,
        Boolean,

        Date,
        Time,
        DateTime,
        DateTimeOffset,

        Array,
        Map,

        Query,
        Command,
        AuthorizeWhenAttribute,
        AuthorizeWhenHasAnyOfAttribute,
        QueryCacheAttribute,
        Attribute,
    }

    public abstract record AttributeArgument
    {
        private AttributeArgument() { }

        [DebuggerDisplay("[{Position}] = {Value}")]
        public sealed record Positional(int Position, ValueRef Value) : AttributeArgument;
        [DebuggerDisplay("[Name] = {Value}")]
        public sealed record Named(string Name, ValueRef Value) : AttributeArgument;
    }

    [DebuggerDisplay("<{Name,nq}>")]
    public sealed record GenericParameter(string Name);
    [DebuggerDisplay("{Value} ({Type})")]
    public sealed record ValueRef(ValueType Type, object Value);
    [DebuggerDisplay("[{Type}({Arguments,results})]")]
    public sealed record AttributeRef(TypeRef Type, ImmutableList<AttributeArgument> Arguments);
    [DebuggerDisplay("{Type} {Name,nq}")]
    public sealed record PropertyRef(TypeRef Type, string Name, ImmutableList<AttributeRef> Attributes, string Comment);
    [DebuggerDisplay("{Name,nq} = {Value}")]
    public sealed record ConstantRef(string Name, ValueRef Value, string Comment);
    [DebuggerDisplay("{Name,nq} = {Value}")]
    public sealed record EnumValue(string Name, long Value, string Comment);

    public abstract record GenericArgument
    {
        private GenericArgument() { }

        [DebuggerDisplay("<{Name,nq}>")]
        public sealed record Param(string Name) : GenericArgument;
        [DebuggerDisplay("<{Ref}>")]
        public sealed record Type(TypeRef Ref) : GenericArgument;
    }

    public abstract record TypeRef
    {
        private TypeRef() { }

        [DebuggerDisplay("<{Name,nq}>")]
        public sealed record Generic(string Name) : TypeRef;
        [DebuggerDisplay("{Name}")]
        public sealed record Internal(string Name, ImmutableList<GenericArgument> GenericArguments) : TypeRef;
        [DebuggerDisplay("({Type})")]
        public sealed record Known(KnownType Type, ImmutableList<GenericArgument> GenericArguments) : TypeRef;
    }

    public abstract record ErrorCode
    {
        private ErrorCode() { }

        [DebuggerDisplay("{Name,nq} = {Value}")]
        public sealed record Single(string Name, int Code) : ErrorCode;
        [DebuggerDisplay("[{Name,nq} ({GroupId,nq})]")]
        public sealed record Group(string Name, string GroupId, ImmutableList<ErrorCode> InnerCodes) : ErrorCode;
    }

    public abstract record Statement
    {
        public string Name { get; }
        public string Comment { get; init; }

        private Statement(string name)
        {
            Name = name;
        }

        [DebuggerDisplay("Enum: {Name,nq}")]
        public sealed record EnumStatement(string Name, ImmutableList<EnumValue> Members) : Statement(Name);

        public abstract record TypeStatement : Statement
        {
            private TypeStatement(string Name) : base(Name) { }

            public ImmutableList<TypeRef> Extends { get; init; }

            public ImmutableList<GenericParameter> GenericParameters { get; init; }
            public ImmutableList<AttributeRef> Attributes { get; init; }

            public ImmutableList<PropertyRef> Properties { get; init; }
            public ImmutableList<ConstantRef> Constants { get; init; }

            [DebuggerDisplay("DTO: {Name,nq}")]
            public sealed record DTOStatement(string Name) : TypeStatement(Name)
            { }

            [DebuggerDisplay("Query: {Name,nq}")]
            public sealed record QueryStatement(string Name) : TypeStatement(Name)
            {
                public TypeRef ReturnType { get; init; }
            }

            [DebuggerDisplay("Command: {Name,nq}")]
            public sealed record CommandStatement(string Name) : TypeStatement(Name)
            {
                public ImmutableList<ErrorCode> ErrorCodes { get; init; }
            }
        }
    }
}
