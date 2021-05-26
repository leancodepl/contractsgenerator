using System;
using System.Collections.Generic;
using System.Linq;
using LeanCode.CQRS;
using LeanCode.CQRS.Security;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LeanCode.ContractsGenerator
{
    public sealed class ContractTypes
    {
        public INamedTypeSymbol QueryType { get; }
        public INamedTypeSymbol CommandType { get; }

        public INamedTypeSymbol AuthorizeWhenAttribute { get; }
        public INamedTypeSymbol AuthorizeWhenHasAnyOfAttribute { get; }
        public INamedTypeSymbol QueryCacheAttribute { get; }

        public INamedTypeSymbol Attribute { get; }
        public INamedTypeSymbol AttributeUsageAttribute { get; }
        public INamedTypeSymbol ReadOnlyDictionary { get; }
        public INamedTypeSymbol Dictionary { get; }

        public ContractTypes(CSharpCompilation compilation)
        {
            QueryType = compilation.GetTypeByMetadataName(typeof(IRemoteQuery<>).FullName).ConstructUnboundGenericType();
            CommandType = compilation.GetTypeByMetadataName(typeof(IRemoteCommand).FullName);

            AuthorizeWhenAttribute = compilation.GetTypeByMetadataName(typeof(AuthorizeWhenAttribute).FullName);
            AuthorizeWhenHasAnyOfAttribute = compilation.GetTypeByMetadataName(typeof(AuthorizeWhenHasAnyOfAttribute).FullName);
            QueryCacheAttribute = compilation.GetTypeByMetadataName(typeof(QueryCacheAttribute).FullName);
            Attribute = compilation.GetTypeByMetadataName(typeof(Attribute).FullName);
            AttributeUsageAttribute = compilation.GetTypeByMetadataName(typeof(AttributeUsageAttribute).FullName);
            ReadOnlyDictionary = compilation.GetTypeByMetadataName(typeof(IReadOnlyDictionary<,>).FullName).ConstructUnboundGenericType();
            Dictionary = compilation.GetTypeByMetadataName(typeof(IDictionary<,>).FullName).ConstructUnboundGenericType();
        }

        public bool IsQuery(ITypeSymbol symbol)
        {
            return
                symbol is INamedTypeSymbol ns &&
                !ns.IsUnboundGenericType &&
                !ns.IsAbstract &&
                ns.AllInterfaces.Any(IsQueryType);
        }

        public bool IsCommand(ITypeSymbol symbol)
        {
            return
                symbol is INamedTypeSymbol ns &&
                !ns.IsUnboundGenericType &&
                !ns.IsAbstract &&
                ns.AllInterfaces.Any(IsCommandType);
        }

        public ITypeSymbol ExtractQueryResult(ITypeSymbol symbol)
        {
            if (IsQuery(symbol))
            {
                var queryType = symbol.AllInterfaces.First(IsQueryType);
                return queryType.TypeArguments[0];
            }
            else if (IsQueryType(symbol))
            {
                return ((INamedTypeSymbol)symbol).TypeArguments[0];
            }
            else
            {
                throw new ArgumentException($"The symbol `{symbol}` is not a query type.");
            }
        }

        public bool IsQueryType(ITypeSymbol i) =>
            i is INamedTypeSymbol ns && ns.IsGenericType && QueryType.Equals(ns.ConstructUnboundGenericType(), SymbolEqualityComparer.Default);

        public bool IsCommandType(ITypeSymbol i) =>
            CommandType.Equals(i, SymbolEqualityComparer.Default);

        public bool IsAuthorizeWhenType(ITypeSymbol i) =>
            AuthorizeWhenAttribute.Equals(i, SymbolEqualityComparer.Default);

        public bool IsAuthorizeWhenHasAnyOfType(ITypeSymbol i) =>
            AuthorizeWhenHasAnyOfAttribute.Equals(i, SymbolEqualityComparer.Default);

        public bool IsQueryCacheType(ITypeSymbol i) =>
            QueryCacheAttribute.Equals(i, SymbolEqualityComparer.Default);

        public bool IsAttributeType(ITypeSymbol i) =>
            Attribute.Equals(i, SymbolEqualityComparer.Default);

        public bool IsAttributeUsageType(ITypeSymbol i) =>
            AttributeUsageAttribute.Equals(i, SymbolEqualityComparer.Default);

        public bool IsReadOnlyDictionary(ITypeSymbol i)
        {
            return
                i is INamedTypeSymbol ns &&
                ns.IsGenericType && (
                    ReadOnlyDictionary.Equals(ns.ConstructUnboundGenericType(), SymbolEqualityComparer.Default) ||
                    Dictionary.Equals(ns.ConstructUnboundGenericType(), SymbolEqualityComparer.Default));
        }
    }
}
