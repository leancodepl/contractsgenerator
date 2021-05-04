using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Google.Protobuf.Collections;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGeneratorV2
{
    internal class ContractsGenerator
    {
        private const string ErrorCodesName = "ErrorCodes";

        private readonly Contracts contracts;

        public ContractsGenerator(Contracts contracts)
        {
            this.contracts = contracts;
        }

        public Export Generate(string path)
        {
            var export = new Export() { BasePath = path };
            contracts.ListAllTypes()
                .Select(ProcessType)
                .Where(s => s is not null)
                .ToList()
                .OrderBy(s => s.Name)
                .SaveToRepeatedField(export.Statements);
            ListKnownGroups(export.Statements)
                .SaveToRepeatedField(export.KnownErrorGroups);
            return export;
        }

        private Statement? ProcessType(INamedTypeSymbol symbol)
        {
            if (IsIgnored(symbol))
            {
                return null;
            }

            var result = new Statement
            {
                Name = ConstructName(symbol),
                Comment = ExtractComments(symbol),
            };
            symbol.TypeParameters
               .Select(ToParam)
               .SaveToRepeatedField(result.GenericParameters);
            symbol.Interfaces
               .Append(symbol.BaseType)
               .Where(s => !IsIgnored(s))
               .Select(ToTypeRef)
               .SaveToRepeatedField(result.Extends);
            AllProperties(symbol)
               .SelectMany(s => s)
               .Select(ToProperty)
               .SaveToRepeatedField(result.Properties);
            symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(fs => fs.HasConstantValue)
                .Select(ToConstant)
                .SaveToRepeatedField(result.Constants);

            MapType(symbol, result);

            return result;

            GenericParameter ToParam(ITypeParameterSymbol ts) =>
                new() { Name = ts.Name };
            ConstantRef ToConstant(IFieldSymbol fs) =>
                new() { Name = fs.Name, Value = ToValueRef(fs.ConstantValue), Comment = ExtractComments(fs) };

            PropertyRef ToProperty(IPropertySymbol ps)
            {
                var res = new PropertyRef
                {
                    Type = ToTypeRef(ps.Type),
                    Name = ps.Name,
                    Comment = ExtractComments(ps)
                };
                GetAttributes(ps, res.Attributes);
                return res;
            }

            IEnumerable<IEnumerable<IPropertySymbol>> AllProperties(INamedTypeSymbol ns)
            {
                var currProps = ns.GetMembers().OfType<IPropertySymbol>();
                if (ns.BaseType is not null)
                {
                    return AllProperties(ns.BaseType).Append(currProps);
                }
                else
                {
                    return new[] { currProps };
                }
            }

            void MapType(INamedTypeSymbol symbol, Statement result)
            {
                if (IsQuery(symbol))
                {
                    result.Query = new()
                    {
                        ReturnType = ExtractQueryResultType(symbol),
                    };
                }
                else if (IsCommand(symbol))
                {
                    result.Command = new();
                    ExtractErrorCodes(symbol, result.Command.ErrorCodes);
                }
                else if (symbol.TypeKind == TypeKind.Enum)
                {
                    result.Enum = new();
                    symbol.GetMembers()
                        .OfType<IFieldSymbol>()
                        .Select(f => new EnumValue()
                        {
                            Name = f.Name,
                            Value = Convert.ToInt64(f.ConstantValue),
                            Comment = ExtractComments(f),
                        })
                        .SaveToRepeatedField(result.Enum.Members);
                }
                else
                {
                    result.Dto = new();
                }
            }
        }

        private TypeRef ToTypeRef(ITypeSymbol symbol)
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
                    var inner = ToTypeRef(ns.TypeArguments[0]);
                    inner.Nullable = true;
                    return inner;
                }
                else
                {
                    var res = new TypeRef()
                    {
                        Internal = new()
                        {
                            Name = ConstructName(symbol),
                        },
                        Nullable = isNullable,
                    };
                    ns.TypeArguments
                        .Select(ToTypeRef)
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
                { SpecialType: SpecialType.System_Single } => New(KnownType.Float),
                { SpecialType: SpecialType.System_Double } => New(KnownType.Double),
                { SpecialType: SpecialType.System_Decimal } => New(KnownType.Decimal),
                { SpecialType: SpecialType.System_Boolean } => New(KnownType.Boolean),
                { SpecialType: SpecialType.System_DateTime } => New(KnownType.DateTime),

                { ContainingNamespace: { Name: "System" }, Name: "DateTimeOffset" } => New(KnownType.DateTimeOffset),
                { ContainingNamespace: { Name: "System" }, Name: "Date" } => New(KnownType.Date),
                { ContainingNamespace: { Name: "System" }, Name: "Time" } => New(KnownType.Time),
                { ContainingNamespace: { Name: "System" }, Name: "Guid" } => New(KnownType.Guid),
                { ContainingNamespace: { Name: "System" }, Name: "Uri" } => New(KnownType.Uri),

                _ when ts is INamedTypeSymbol ns && IsQueryType(ns) =>
                    New(KnownType.Query, ToTypeRef(ns.TypeArguments[0])),
                _ when ts is INamedTypeSymbol ns && IsCommandType(ns) =>
                    New(KnownType.Command),
                _ when ts.Equals(contracts.AuthorizeWhenAttribute, SymbolEqualityComparer.Default) =>
                    New(KnownType.AuthorizeWhenAttribute),
                _ when ts.Equals(contracts.AuthorizeWhenHasAnyOfAttribute, SymbolEqualityComparer.Default) =>
                    New(KnownType.AuthorizeWhenHasAnyOfAttribute),
                _ when ts.Equals(contracts.QueryCacheAttribute, SymbolEqualityComparer.Default) =>
                    New(KnownType.QueryCacheAttribute),
                _ when ts.Equals(contracts.Attribute, SymbolEqualityComparer.Default) =>
                    New(KnownType.Attribute),

                IArrayTypeSymbol arr => New(KnownType.Array, ToTypeRef(arr.ElementType)),
                _ when ts is INamedTypeSymbol ns && ns.Arity == 1 && ns.Interfaces.Any(i => i.SpecialType == SpecialType.System_Collections_IEnumerable) =>
                    New(KnownType.Array, ToTypeRef(ns.TypeArguments[0])),

                _ when ts is INamedTypeSymbol ns && ns.Arity == 2 && ns.Interfaces.Any(i => i.Name == "IReadOnlyDictionary") =>
                    New(KnownType.Array, ToTypeRef(ns.TypeArguments[0]), ToTypeRef(ns.TypeArguments[1])),

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

        private bool IsIgnored(INamedTypeSymbol? symbol)
        {
            return symbol is null ||
                symbol.SpecialType == SpecialType.System_Object ||
                symbol.SpecialType == SpecialType.System_Enum ||
                IsErrorCode(symbol) ||
                symbol.Equals(contracts.Attribute, SymbolEqualityComparer.Default) ||
                symbol.Equals(contracts.AttributeUsageAttribute, SymbolEqualityComparer.Default);

            static bool IsErrorCode(ISymbol? sym)
            {
                return sym.Name == ErrorCodesName || (sym.ContainingSymbol is not null && IsErrorCode(sym.ContainingSymbol));
            }
        }

        private bool IsQuery(INamedTypeSymbol symbol)
        {
            return
                !symbol.IsUnboundGenericType &&
                !symbol.IsAbstract &&
                symbol.AllInterfaces.Any(IsQueryType);
        }

        private bool IsCommand(INamedTypeSymbol symbol)
        {
            return
                !symbol.IsUnboundGenericType &&
                !symbol.IsAbstract &&
                symbol.AllInterfaces.Any(IsCommandType);
        }

        private bool IsQueryType(INamedTypeSymbol i)
        {
            return i.IsGenericType && contracts.QueryType.Equals(i.ConstructUnboundGenericType(), SymbolEqualityComparer.Default);
        }

        private bool IsCommandType(INamedTypeSymbol i)
        {
            return contracts.CommandType.Equals(i, SymbolEqualityComparer.Default);
        }

        private TypeRef ExtractQueryResultType(INamedTypeSymbol symbol)
        {
            var queryType = symbol.AllInterfaces.First(i =>
                i.IsGenericType &&
                contracts.QueryType.Equals(i.ConstructUnboundGenericType(), SymbolEqualityComparer.Default));
            return ToTypeRef(queryType.TypeArguments[0]);
        }

        private void ExtractErrorCodes(INamedTypeSymbol symbol, RepeatedField<ErrorCode> output)
        {
            var errCodes = symbol.GetMembers()
                .OfType<INamedTypeSymbol>()
                .Where(s => s.Name == ErrorCodesName)
                .SingleOrDefault();
            if (errCodes is not null)
            {
                MapCodes(errCodes, output);
            }

            void MapCodes(INamedTypeSymbol errCodes, RepeatedField<ErrorCode> output)
            {
                var consts = errCodes
                    .GetMembers()
                    .OfType<IFieldSymbol>()
                    .Select(ToSingleCode);
                var groups = errCodes
                    .GetMembers()
                    .OfType<INamedTypeSymbol>()
                    .Select(ToGroupCode);
                consts.Concat(groups).SaveToRepeatedField(output);
            }

            static ErrorCode ToSingleCode(IFieldSymbol f)
            {
                if (!f.HasConstantValue)
                {
                    throw new InvalidOperationException("The error codes class can only contain constant numeric fields & derived types.");
                }
                return new()
                {
                    Single = new()
                    {
                        Name = f.Name,
                        Code = Convert.ToInt32(f.ConstantValue)
                    }
                };
            }

            ErrorCode ToGroupCode(INamedTypeSymbol ns)
            {
                if (ns.BaseType?.Name != ErrorCodesName)
                {
                    throw new InvalidOperationException($"The base class for error codes group needs to be named `{ErrorCodesName}`.");
                }
                var g = new ErrorCode.Types.Group
                {
                    Name = ns.Name,
                    GroupId = ConstructName(ns.BaseType),
                };
                MapCodes(ns.BaseType, g.InnerCodes);
                return new() { Group = g };
            }
        }

        private void GetAttributes(ISymbol symbol, RepeatedField<AttributeRef> output)
        {
            symbol.GetAttributes()
                .Where(a => !IsIgnored(a.AttributeClass))
                .Select(ToAttribute)
                .SaveToRepeatedField(output);

            AttributeRef ToAttribute(AttributeData a)
            {
                var type = ConstructName(a.AttributeClass);
                var positional = a.ConstructorArguments
                    .SelectMany(a => a.Kind == TypedConstantKind.Array ? a.Values.Select(v => v.Value) : new[] { a.Value })
                    .Select((v, i) => new AttributeArgument() { Positional = new() { Position = i, Value = ToValueRef(v) } })
                    .Cast<AttributeArgument>();
                var named = a.NamedArguments
                    .SelectMany(a => a.Value.Kind == TypedConstantKind.Array ? a.Value.Values.Select(v => (a.Key, v.Value)) : new[] { (a.Key, a.Value.Value) })
                    .Select((v, i) => new AttributeArgument() { Named = new() { Name = v.Key, Value = ToValueRef(v) } })
                    .Cast<AttributeArgument>();
                var result = new AttributeRef { AttributeName = type };
                positional.Concat(named).SaveToRepeatedField(result.Argument);
                return result;
            }
        }

        private static ValueRef ToValueRef(object? val)
        {
            return val switch
            {
                null => new() { Null = new() },
                byte v => new ValueRef { Number = new() { Value = v } },
                sbyte v => new ValueRef { Number = new() { Value = v } },
                int v => new ValueRef { Number = new() { Value = v } },
                long v => new ValueRef { Number = new() { Value = v } },
                short v => new ValueRef { Number = new() { Value = v } },
                ushort v => new ValueRef { Number = new() { Value = v } },
                uint v => new ValueRef { Number = new() { Value = v } },
                ulong v => new ValueRef { Number = new() { Value = (long)v } },
                float v => new ValueRef { FloatingPoint = new() { Value = v } },
                double v => new ValueRef { FloatingPoint = new() { Value = v } },
                string v => new ValueRef { String = new() { Value = v } },
                bool v => new ValueRef { Bool = new() { Value = v } },
                _ => throw new NotSupportedException($"Cannot geenrate contracts for constant of type {val.GetType()}."),
            };
        }

        private static string ConstructName(ISymbol symbol)
        {
            var sb = new StringBuilder();
            Construct(symbol, sb);
            return sb.Remove(sb.Length - 1, 1).ToString();

            static void Construct(ISymbol symbol, StringBuilder sb)
            {
                if (symbol.ContainingType is null)
                {
                    ConstructNS(symbol.ContainingNamespace, sb);
                }
                else
                {
                    Construct(symbol.ContainingType, sb);
                }

                sb.Append(symbol.Name).Append('.');
            }

            static void ConstructNS(INamespaceSymbol symbol, StringBuilder sb)
            {
                if (!symbol.IsGlobalNamespace)
                {
                    ConstructNS(symbol.ContainingNamespace, sb);
                    sb.Append(symbol.Name).Append('.');
                }
            }
        }

        private static string ExtractComments(ISymbol symbol)
        {
            var xml = symbol.GetDocumentationCommentXml();
            if (!string.IsNullOrEmpty(xml))
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var sb = new StringBuilder();
                foreach (var t in FlattenAllNodes(doc.DocumentElement))
                {
                    sb.AppendLine(t.InnerText.Trim());
                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }

            IEnumerable<XmlNode> FlattenAllNodes(XmlNode n)
            {
                if (n.NodeType == XmlNodeType.Text)
                {
                    yield return n;
                }

                foreach (var c in n.ChildNodes.Cast<XmlNode>().SelectMany(FlattenAllNodes))
                {
                    yield return c;
                }
            }
        }

        private static IEnumerable<ErrorCode.Types.Group> ListKnownGroups(IEnumerable<Statement> statements)
        {
            return statements
                .Where(s => s.Command is not null)
                .SelectMany(c => ListGroups(c.Command.ErrorCodes));

            static IEnumerable<ErrorCode.Types.Group> ListGroups(IEnumerable<ErrorCode> gs)
            {
                foreach (var g in gs.OfType<ErrorCode.Types.Group>())
                {
                    yield return g;

                    foreach (var i in ListGroups(g.InnerCodes))
                    {
                        yield return i;
                    }
                }
            }
        }

        private static bool IsNullable(ITypeSymbol symbol)
        {
            return symbol.NullableAnnotation == NullableAnnotation.Annotated;
        }
    }

    internal static class IEnumerableExtensions
    {
        public static RepeatedField<T> ToRepeatedField<T>(this IEnumerable<T> src)
        {
            var f = new RepeatedField<T>();
            f.AddRange(src);
            return f;
        }

        public static void SaveToRepeatedField<T>(this IEnumerable<T> src, RepeatedField<T> output)
        {
            output.AddRange(src);
        }
    }
}
