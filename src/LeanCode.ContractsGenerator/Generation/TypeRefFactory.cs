using LeanCode.ContractsGenerator.Compilation;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator.Generation;

public sealed class TypeRefFactory
{
    private readonly CompiledContracts contracts;

    public TypeRefFactory(CompiledContracts contracts)
    {
        this.contracts = contracts;
    }

    public TypeRef From(ITypeSymbol symbol)
    {
        var isNullable = IsNullable(symbol);

        if (TryKnownTypeRef(symbol) is TypeRef.Types.Known k)
        {
            return new TypeRef
            {
                Known = k,
                Nullable = isNullable,
            };
        }
        else if (symbol is INamedTypeSymbol ns)
        {
            if (ns.OriginalDefinition?.SpecialType == SpecialType.System_Nullable_T)
            {
                var inner = From(ns.TypeArguments[0]);
                inner.Nullable = true;
                return inner;
            }
            else
            {
                var res = new TypeRef()
                {
                    Internal = new()
                    {
                        Name = symbol.ToFullName(),
                    },
                    Nullable = isNullable,
                };
                ns.TypeArguments
                    .Select(From)
                    .SaveToRepeatedField(res.Internal.Arguments);
                return res;
            }
        }
        else if (symbol is ITypeParameterSymbol ts)
        {
            return new()
            {
                Generic = new() { Name = ts.Name },
                Nullable = isNullable,
            };
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private TypeRef.Types.Known? TryKnownTypeRef(ITypeSymbol ts)
    {
        return ts switch
        {
            { SpecialType: SpecialType.System_Object } => New(KnownType.Object),
            { SpecialType: SpecialType.System_String } => New(KnownType.String),
            { SpecialType: SpecialType.System_SByte } => New(KnownType.Int8),
            { SpecialType: SpecialType.System_Byte } => New(KnownType.Uint8),
            { SpecialType: SpecialType.System_Int16 } => New(KnownType.Int16),
            { SpecialType: SpecialType.System_UInt16 } => New(KnownType.Uint16),
            { SpecialType: SpecialType.System_Int32 } => New(KnownType.Int32),
            { SpecialType: SpecialType.System_UInt32 } => New(KnownType.Uint32),
            { SpecialType: SpecialType.System_Int64 } => New(KnownType.Int64),
            { SpecialType: SpecialType.System_UInt64 } => New(KnownType.Uint64),
            { SpecialType: SpecialType.System_Single } => New(KnownType.Float32),
            { SpecialType: SpecialType.System_Double } => New(KnownType.Float64),
            { SpecialType: SpecialType.System_Boolean } => New(KnownType.Boolean),
            { ContainingNamespace.Name: "System", Name: "DateTimeOffset" } => New(KnownType.DateTimeOffset),
            { ContainingNamespace.Name: "System", Name: "DateOnly" } => New(KnownType.DateOnly),
            { ContainingNamespace.Name: "System", Name: "TimeOnly" } => New(KnownType.TimeOnly),
            { ContainingNamespace.Name: "System", Name: "Guid" } => New(KnownType.Guid),
            { ContainingNamespace.Name: "System", Name: "Uri" } => New(KnownType.Uri),
            { ContainingNamespace.Name: "System", Name: "TimeSpan" } => New(KnownType.TimeSpan),
            { ContainingNamespace: { Name: "Contracts", ContainingNamespace.Name: "LeanCode" }, Name: "Binary" } => New(KnownType.Binary),
            { ContainingNamespace: { Name: "Contracts", ContainingNamespace.Name: "LeanCode" }, Name: "CommandResult" } =>
                New(KnownType.CommandResult),

            _ when contracts.Types.IsQueryType(ts) =>
                New(KnownType.Query, From(contracts.Types.ExtractQueryResult(ts))),
            _ when ts is INamedTypeSymbol ns && contracts.Types.IsCommandType(ns) =>
                New(KnownType.Command),
            _ when contracts.Types.IsOperationType(ts) =>
                New(KnownType.Operation, From(contracts.Types.ExtractOperationResult(ts))),
            _ when contracts.Types.IsAuthorizeWhenType(ts) =>
                New(KnownType.AuthorizeWhenAttribute),
            _ when contracts.Types.IsAuthorizeWhenHasAnyOfType(ts) =>
                New(KnownType.AuthorizeWhenHasAnyOfAttribute),
            _ when contracts.Types.IsAttributeType(ts) =>
                New(KnownType.Attribute),

            IArrayTypeSymbol arr => New(KnownType.Array, From(arr.ElementType)),
            _ when ts is INamedTypeSymbol ns && ns.Arity == 1 && ns.Interfaces.Any(i => i.SpecialType == SpecialType.System_Collections_IEnumerable) =>
                New(KnownType.Array, From(ns.TypeArguments[0])),

            _ when ts is INamedTypeSymbol ns && ns.Arity == 2 &&
                (contracts.Types.IsReadOnlyDictionary(ns) || ns.Interfaces.Any(i => contracts.Types.IsReadOnlyDictionary(i))) =>
                New(KnownType.Map, From(ns.TypeArguments[0]), From(ns.TypeArguments[1])),

            _ => null,
        };

        static TypeRef.Types.Known New(KnownType type, params TypeRef[] args)
        {
            var res = new TypeRef.Types.Known
            {
                Type = type,
            };
            args.SaveToRepeatedField(res.Arguments);
            return res;
        }
    }

    private static bool IsNullable(ITypeSymbol symbol)
    {
        return symbol.NullableAnnotation == NullableAnnotation.Annotated;
    }
}
