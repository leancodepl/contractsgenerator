using LeanCode.CQRS;
using LeanCode.CQRS.Security;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#pragma warning disable RS1024

namespace LeanCode.ContractsGenerator
{
    public sealed class ContractTypes
    {
        private HashSet<INamedTypeSymbol> QueryType { get; }
        private HashSet<INamedTypeSymbol> CommandType { get; }

        private HashSet<INamedTypeSymbol> AuthorizeWhenAttribute { get; }
        private HashSet<INamedTypeSymbol> AuthorizeWhenHasAnyOfAttribute { get; }
        private HashSet<INamedTypeSymbol> QueryCacheAttribute { get; }

        private HashSet<INamedTypeSymbol> ExcludeFromContractsGenerationAttribute { get; }

        private HashSet<INamedTypeSymbol> Attribute { get; }
        private HashSet<INamedTypeSymbol> AttributeUsageAttribute { get; }
        private HashSet<INamedTypeSymbol> ReadOnlyDictionary { get; }
        private HashSet<INamedTypeSymbol> Dictionary { get; }

        public ContractTypes(IReadOnlyCollection<CSharpCompilation> compilations)
        {
            QueryType = GetUnboundTypeSymbols(compilations, typeof(IRemoteQuery<>));
            CommandType = GetTypeSymbols<IRemoteCommand>(compilations);

            AuthorizeWhenAttribute = GetTypeSymbols<AuthorizeWhenAttribute>(compilations);
            AuthorizeWhenHasAnyOfAttribute = GetTypeSymbols<AuthorizeWhenHasAnyOfAttribute>(compilations);
            QueryCacheAttribute = GetTypeSymbols<QueryCacheAttribute>(compilations);
            ExcludeFromContractsGenerationAttribute = GetTypeSymbols<ExcludeFromContractsGenerationAttribute>(compilations);
            Attribute = GetTypeSymbols<Attribute>(compilations);
            AttributeUsageAttribute = GetTypeSymbols<AttributeUsageAttribute>(compilations);
            ReadOnlyDictionary = GetUnboundTypeSymbols(compilations, typeof(IReadOnlyDictionary<,>));
            Dictionary = GetUnboundTypeSymbols(compilations, typeof(IDictionary<,>));
        }

        private static HashSet<INamedTypeSymbol> GetTypeSymbols<T>(
            IReadOnlyCollection<CSharpCompilation> compilations)
        {
            var name = typeof(T).FullName!;
            var result = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            foreach (var c in compilations)
            {
                var type = c.GetTypeByMetadataName(name);
                if (type is null)
                {
                    throw new GenerationFailedException($"Cannot locate type {name} in compilation unit `{c.AssemblyName ?? "UNKNOWN"}`.");
                }

                result.Add(type);
            }

            return result;
        }

        private static HashSet<INamedTypeSymbol> GetUnboundTypeSymbols(
            IReadOnlyCollection<CSharpCompilation> compilations,
            Type type)
        {
            var name = type.FullName!;
            var result = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            foreach (var c in compilations)
            {
                var t = c.GetTypeByMetadataName(name)?.ConstructUnboundGenericType();
                if (t is null)
                {
                    throw new GenerationFailedException($"Cannot locate generic type {name} in compilation unit `{c.AssemblyName ?? "UNKNOWN"}`.");
                }

                result.Add(t);
            }

            return result;
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
            i is INamedTypeSymbol ns && ns.IsGenericType && QueryType.Contains(ns.ConstructUnboundGenericType());

        public bool IsCommandType(ITypeSymbol i) =>
            CommandType.Contains(i);

        public bool IsAuthorizeWhenType(ITypeSymbol i) =>
            AuthorizeWhenAttribute.Contains(i);

        public bool IsAuthorizeWhenHasAnyOfType(ITypeSymbol i) =>
            AuthorizeWhenHasAnyOfAttribute.Contains(i);

        public bool IsQueryCacheType(ITypeSymbol i) =>
            QueryCacheAttribute.Contains(i);

        public bool IsExcludeFromContractsGenerationType(ITypeSymbol? i) =>
            ExcludeFromContractsGenerationAttribute.Contains(i);

        public bool IsAttributeType(ITypeSymbol i) =>
            Attribute.Contains(i);

        public bool IsAttributeUsageType(ITypeSymbol i) =>
            AttributeUsageAttribute.Contains(i);

        public bool IsReadOnlyDictionary(ITypeSymbol i)
        {
            return
                i is INamedTypeSymbol ns &&
                ns.IsGenericType && (
                    ReadOnlyDictionary.Contains(ns.ConstructUnboundGenericType()) ||
                    Dictionary.Contains(ns.ConstructUnboundGenericType()));
        }
    }
}
