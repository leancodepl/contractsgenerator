using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml;
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

        public ImmutableList<Statement> Generate()
        {
            return contracts.ListAllTypes()
                .Select(Process)
                .Where(s => s is not null)
                .ToList()
                .OrderBy(s => s.Name)
                .ToImmutableList();
        }

        private Statement? Process(INamedTypeSymbol symbol)
        {
            return symbol.TypeKind switch
            {
                TypeKind.Enum => ProcessEnum(symbol),
                TypeKind.Interface => ProcessType(symbol),
                TypeKind.Class => ProcessType(symbol),
                TypeKind.Struct => ProcessType(symbol),
                _ => null,
            };
        }

        private Statement ProcessEnum(INamedTypeSymbol symbol)
        {
            var name = ConstructName(symbol);
            var membs = symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Select(f => new EnumValue(f.Name, Convert.ToInt64(f.ConstantValue), ExtractComments(f)))
                .ToImmutableList();
            return new Statement.EnumStatement(name, membs);
        }

        private Statement? ProcessType(INamedTypeSymbol symbol)
        {
            if (IsIgnored(symbol))
            {
                return null;
            }

            var name = ConstructName(symbol);
            var comment = ExtractComments(symbol);
            var genericParams = symbol.TypeParameters
                .Select(ToParam)
                .ToImmutableList();
            var attributes = GetAttributes(symbol);
            var extends = symbol.Interfaces
                .Append(symbol.BaseType)
                .Where(s => !IsIgnored(s))
                .Select(ToTypeRef)
                .ToImmutableList();
            var properties = AllProperties(symbol)
                .SelectMany(s => s)
                .Select(ToProperty)
                .ToImmutableList();
            var constants = symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(fs => fs.HasConstantValue)
                .Select(ToConstant)
                .ToImmutableList();

            if (IsQuery(symbol))
            {
                return new Statement.TypeStatement.QueryStatement(name)
                {
                    Comment = comment,
                    GenericParameters = genericParams,
                    Attributes = attributes,
                    Extends = extends,
                    Properties = properties,
                    Constants = constants,

                    ReturnType = ExtractQueryResultType(symbol),
                };
            }
            else if (IsCommand(symbol))
            {
                return new Statement.TypeStatement.CommandStatement(name)
                {
                    Comment = comment,
                    GenericParameters = genericParams,
                    Attributes = attributes,
                    Extends = extends,
                    Properties = properties,
                    Constants = constants,

                    ErrorCodes = ExtractErrorCodes(symbol),
                };
            }
            else
            {
                return new Statement.TypeStatement.DTOStatement(name)
                {
                    Comment = comment,
                    GenericParameters = genericParams,
                    Attributes = attributes,
                    Extends = extends,
                    Properties = properties,
                    Constants = constants,
                };
            }

            GenericParameter ToParam(ITypeParameterSymbol ts) =>
                new(ts.Name);
            ConstantRef ToConstant(IFieldSymbol fs) =>
                new(fs.Name, ToValueRef(fs.ConstantValue), ExtractComments(fs));
            PropertyRef ToProperty(IPropertySymbol ps) =>
                new(ToTypeRef(ps.Type), ps.Name, GetAttributes(ps), ExtractComments(ps));

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
        }

        private TypeRef ToTypeRef(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol ns)
            {
                if (TryKnownTypeRef(ns) is TypeRef r)
                {
                    return r;
                }
                else
                {
                    var name = ConstructName(symbol);
                    return new TypeRef.Internal(name, ns.TypeArguments.Select(ToGenericArg).ToImmutableList());
                }
            }
            else if (symbol is ITypeParameterSymbol ts)
            {
                return new TypeRef.Generic(ts.Name);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private TypeRef.Known? TryKnownTypeRef(ITypeSymbol ts)
        {
            return ts switch
            {
                { SpecialType: SpecialType.System_Object } => new(KnownType.Object, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_String } => new(KnownType.String, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_SByte } => new(KnownType.Int8, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_Byte } => new(KnownType.UInt8, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_Int16 } => new(KnownType.Int16, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_UInt16 } => new(KnownType.UInt16, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_Int32 } => new(KnownType.Int32, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_UInt32 } => new(KnownType.UInt32, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_Int64 } => new(KnownType.Int64, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_UInt64 } => new(KnownType.UInt64, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_Single } => new(KnownType.Float, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_Double } => new(KnownType.Double, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_Decimal } => new(KnownType.Decimal, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_Boolean } => new(KnownType.Boolean, ImmutableList<GenericArgument>.Empty),
                { SpecialType: SpecialType.System_DateTime } => new(KnownType.DateTime, ImmutableList<GenericArgument>.Empty),

                { ContainingNamespace: { Name: "System" }, Name: "DateTimeOffset" } => new(KnownType.DateTimeOffset, ImmutableList<GenericArgument>.Empty),
                { ContainingNamespace: { Name: "System" }, Name: "Date" } => new(KnownType.Date, ImmutableList<GenericArgument>.Empty),
                { ContainingNamespace: { Name: "System" }, Name: "Time" } => new(KnownType.Time, ImmutableList<GenericArgument>.Empty),
                { ContainingNamespace: { Name: "System" }, Name: "Guid" } => new(KnownType.Guid, ImmutableList<GenericArgument>.Empty),
                { ContainingNamespace: { Name: "System" }, Name: "Uri" } => new(KnownType.Uri, ImmutableList<GenericArgument>.Empty),

                _ when ts is INamedTypeSymbol ns && IsQueryType(ns) =>
                    new(KnownType.Query, ImmutableList.Create(ToGenericArg(ns.TypeArguments[0]))),
                _ when ts is INamedTypeSymbol ns && IsCommandType(ns) =>
                    new(KnownType.Command, ImmutableList<GenericArgument>.Empty),
                _ when ts.Equals(contracts.AuthorizeWhenAttribute, SymbolEqualityComparer.Default) =>
                    new(KnownType.AuthorizeWhenAttribute, ImmutableList<GenericArgument>.Empty),
                _ when ts.Equals(contracts.AuthorizeWhenHasAnyOfAttribute, SymbolEqualityComparer.Default) =>
                    new(KnownType.AuthorizeWhenHasAnyOfAttribute, ImmutableList<GenericArgument>.Empty),
                _ when ts.Equals(contracts.QueryCacheAttribute, SymbolEqualityComparer.Default) =>
                    new(KnownType.QueryCacheAttribute, ImmutableList<GenericArgument>.Empty),
                _ when ts.Equals(contracts.Attribute, SymbolEqualityComparer.Default) =>
                    new(KnownType.Attribute, ImmutableList<GenericArgument>.Empty),

                IArrayTypeSymbol arr => new(KnownType.Array, ImmutableList.Create(ToGenericArg(arr.ElementType))),
                _ when ts is INamedTypeSymbol ns && ns.Arity == 1 && ns.Interfaces.Any(i => i.SpecialType == SpecialType.System_Collections_IEnumerable) =>
                    new(KnownType.Array, ImmutableList.Create(ToGenericArg(ns.TypeArguments[0]))),

                _ when ts is INamedTypeSymbol ns && ns.Arity == 2 && ns.Interfaces.Any(i => i.Name == "IReadOnlyDictionary") =>
                    new(KnownType.Array, ImmutableList.Create(ToGenericArg(ns.TypeArguments[0]), ToGenericArg(ns.TypeArguments[1]))),

                _ => null,
            };
        }

        private GenericArgument ToGenericArg(ITypeSymbol ts)
        {
            if (ts is INamedTypeSymbol named)
            {
                return new GenericArgument.Type(ToTypeRef(named));
            }
            else
            {
                return new GenericArgument.Param(ts.Name);
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

        private ImmutableList<ErrorCode> ExtractErrorCodes(INamedTypeSymbol symbol)
        {
            var errCodes = symbol.GetMembers()
                .OfType<INamedTypeSymbol>()
                .Where(s => s.Name == ErrorCodesName)
                .SingleOrDefault();
            if (errCodes is not null)
            {
                return MapCodes(errCodes);
            }
            else
            {
                return ImmutableList<ErrorCode>.Empty;
            }

            ImmutableList<ErrorCode> MapCodes(INamedTypeSymbol errCodes)
            {
                var consts = errCodes
                    .GetMembers()
                    .OfType<IFieldSymbol>()
                    .Select(ToSingleCode);
                var groups = errCodes
                    .GetMembers()
                    .OfType<INamedTypeSymbol>()
                    .Select(ToGroupCode);
                return consts.Concat(groups).ToImmutableList();
            }

            static ErrorCode ToSingleCode(IFieldSymbol f)
            {
                if (!f.HasConstantValue)
                {
                    throw new InvalidOperationException("The error codes class can only contain constant numeric fields & derived types.");
                }
                return new ErrorCode.Single(f.Name, Convert.ToInt32(f.ConstantValue));
            }

            ErrorCode ToGroupCode(INamedTypeSymbol ns)
            {
                if (ns.BaseType?.Name != ErrorCodesName)
                {
                    throw new InvalidOperationException($"The base class for error codes group needs to be named `{ErrorCodesName}`.");
                }
                return new ErrorCode.Group(ns.Name, ConstructName(ns.BaseType), MapCodes(ns.BaseType));
            }
        }

        private ImmutableList<AttributeRef> GetAttributes(ISymbol symbol)
        {
            return symbol.GetAttributes()
                .Where(a => !IsIgnored(a.AttributeClass))
                .Select(ToAttribute)
                .ToImmutableList();

            AttributeRef ToAttribute(AttributeData a)
            {
                var type = ConstructName(a.AttributeClass);
                var positional = a.ConstructorArguments
                    .SelectMany(a => a.Kind == TypedConstantKind.Array ? a.Values.Select(v => v.Value) : new[] { a.Value })
                    .Select((v, i) => new AttributeArgument.Positional(i, ToValueRef(v)))
                    .Cast<AttributeArgument>();
                var named = a.NamedArguments
                    .SelectMany(a => a.Value.Kind == TypedConstantKind.Array ? a.Value.Values.Select(v => (a.Key, v.Value)) : new[] { (a.Key, a.Value.Value) })
                    .Select(v => new AttributeArgument.Named(v.Key, ToValueRef(v.Value)))
                    .Cast<AttributeArgument>();
                var args = positional.Concat(named).ToImmutableList();
                return new AttributeRef(type, args);
            }
        }

        private static ValueRef ToValueRef(object? val)
        {
            return val switch
            {
                null => new(ValueType.Null, null),
                byte v => new(ValueType.Number, (long)v),
                sbyte v => new(ValueType.Number, (long)v),
                int v => new(ValueType.Number, (long)v),
                long v => new(ValueType.Number, (long)v),
                short v => new(ValueType.Number, (long)v),
                ushort v => new(ValueType.Number, (long)v),
                uint v => new(ValueType.Number, (long)v),
                ulong v => new(ValueType.Number, (long)v),
                float v => new(ValueType.FloatingPointNumber, (double)v),
                double v => new(ValueType.FloatingPointNumber, (double)v),
                string v => new(ValueType.String, v),
                bool v => new(ValueType.Boolean, v),
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
    }
}
