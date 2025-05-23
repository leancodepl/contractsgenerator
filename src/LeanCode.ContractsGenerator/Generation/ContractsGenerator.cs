using System.Globalization;
using Google.Protobuf.Collections;
using LeanCode.ContractsGenerator.Compilation;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator.Generation;

public class ContractsGenerator(CompiledContracts contracts, GeneratorConfiguration? configuration = null)
{
    private readonly CompiledContracts contracts = contracts;
    private readonly GeneratorConfiguration configuration = configuration ?? GeneratorConfiguration.Default;

    private readonly TypeRefFactory typeRef = new(contracts);

    public Export Generate()
    {
        var export = GenerateCore();
        Analyze(export);
        return export;
    }

    public Export Generate(List<Export> externalContracts, bool excludeExternalContractsFromOutput)
    {
        var export = GenerateCore();

        var merged = externalContracts.Count > 0 ? new(export) : export;

        foreach (var e in externalContracts)
        {
            merged.MergeFrom(e);
        }

        merged.ProjectName = export.ProjectName;
        Analyze(merged);

        return excludeExternalContractsFromOutput ? export : merged;
    }

    private Export GenerateCore()
    {
        var export = new Export() { ProjectName = contracts.ProjectName };
        contracts
            .ListAllTypes()
            .Select(ProcessType)
            .Where(s => s is not null)
            .Cast<Statement>()
            .ToList()
            .OrderBy(s => s.Name)
            .SaveToRepeatedField(export.Statements);
        ErrorCodes.ListKnownGroups(export.Statements).SaveToRepeatedField(export.KnownErrorGroups);
        return export;
    }

    private void Analyze(Export export)
    {
        var errors = new Analyzers.AllAnalyzers(configuration).Analyze(export).ToList();

        if (errors.Count > 0)
        {
            throw new AnalyzeFailedException(errors);
        }
    }

    private Statement? ProcessType(INamedTypeSymbol? symbol)
    {
        if (IsIgnored(symbol) || IsExcluded(symbol))
        {
            return null;
        }

        var result = new Statement { Name = symbol.ToFullName(), Comment = symbol.GetComments() };
        MapType(symbol, result);
        GetAttributes(symbol, result.Attributes);
        return result;

        void MapType(INamedTypeSymbol symbol, Statement result)
        {
            if (contracts.Types.IsQuery(symbol))
            {
                result.Query = new()
                {
                    TypeDescriptor = MapTypeDescriptor(symbol),
                    ReturnType = typeRef.From(contracts.Types.ExtractQueryResult(symbol)),
                };
            }
            else if (contracts.Types.IsCommand(symbol))
            {
                result.Command = new() { TypeDescriptor = MapTypeDescriptor(symbol) };
                ErrorCodes.Extract(symbol).SaveToRepeatedField(result.Command.ErrorCodes);
            }
            else if (contracts.Types.IsOperation(symbol))
            {
                result.Operation = new()
                {
                    TypeDescriptor = MapTypeDescriptor(symbol),
                    ReturnType = typeRef.From(contracts.Types.ExtractOperationResult(symbol)),
                };
            }
            else if (contracts.Types.IsTopic(symbol))
            {
                result.Topic = new() { TypeDescriptor = MapTypeDescriptor(symbol) };
                contracts
                    .Types.ExtractTopicNotifications(symbol)
                    .Select(typeRef.FromNotification)
                    .SaveToRepeatedField(result.Topic.Notifications);
            }
            else if (symbol.TypeKind == TypeKind.Enum)
            {
                result.Enum = new();
                symbol
                    .GetMembers()
                    .OfType<IFieldSymbol>()
                    .Where(s => !IsExcluded(s))
                    .Select(ToEnumValue)
                    .SaveToRepeatedField(result.Enum.Members);
            }
            else
            {
                result.Dto = new() { TypeDescriptor = MapTypeDescriptor(symbol) };
            }
        }
    }

    private TypeDescriptor MapTypeDescriptor(INamedTypeSymbol symbol)
    {
        var descriptor = new TypeDescriptor();
        symbol.TypeParameters.Select(ToParam).SaveToRepeatedField(descriptor.GenericParameters);
        symbol
            .Interfaces.Append(symbol.BaseType)
            .Where(IsNotIgnored)
            .Select(typeRef.From!)
            .SaveToRepeatedField(descriptor.Extends);
        MapProperties(symbol, descriptor);
        symbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(fs => fs.HasConstantValue && !IsExcluded(fs))
            .Select(ToConstant)
            .SaveToRepeatedField(descriptor.Constants);
        return descriptor;

        void MapProperties(INamedTypeSymbol symbol, TypeDescriptor descriptor)
        {
            var baseProps = GatherBaseProperties(symbol);
            symbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(s => !IsExcluded(s) && !AlreadyImplemented(s, baseProps))
                .Select(ToProperty)
                .SaveToRepeatedField(descriptor.Properties);
        }
    }

    private bool IsNotIgnored([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] INamedTypeSymbol? symbol) =>
        !IsIgnored(symbol);

    private bool IsIgnored([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] INamedTypeSymbol? symbol) =>
        symbol is null
        || symbol.SpecialType == SpecialType.System_Object
        || symbol.SpecialType == SpecialType.System_ValueType
        || symbol.SpecialType == SpecialType.System_Enum
        || ErrorCodes.IsErrorCode(symbol)
        || contracts.Types.IsProduceNotificationType(symbol)
        || contracts.Types.IsAttributeUsageType(symbol)
        || contracts.Types.IsRecordEquatable(symbol);

    private bool IsExcluded(ISymbol symbol) =>
        (symbol is IPropertySymbol ps && ContractTypes.IsRecordEqualityContract(ps))
        || symbol.GetAttributes().Any(a => contracts.Types.IsExcludeFromContractsGenerationType(a.AttributeClass));

    private static HashSet<string> GatherBaseProperties(INamedTypeSymbol ns) =>
        [.. ns.AllInterfaces.SelectMany(i => i.GetMembers()).OfType<IPropertySymbol>().Select(p => p.Name)];

    private static bool AlreadyImplemented(IPropertySymbol prop, HashSet<string> baseProps) =>
        baseProps.Contains(prop.Name);

    private GenericParameter ToParam(ITypeParameterSymbol ts) => new() { Name = ts.Name };

    private ConstantRef ToConstant(IFieldSymbol fs) =>
        new()
        {
            Name = fs.Name,
            Value = fs.ConstantValue.ToValueRef(),
            Comment = fs.GetComments(),
        };

    private PropertyRef ToProperty(IPropertySymbol ps)
    {
        var res = new PropertyRef
        {
            Type = typeRef.From(ps.Type),
            Name = ps.Name,
            Comment = ps.GetComments(),
        };
        GetAttributes(ps, res.Attributes);
        return res;
    }

    private EnumValue ToEnumValue(IFieldSymbol f)
    {
        var res = new EnumValue
        {
            Name = f.Name,
            Value = Convert.ToInt64(f.ConstantValue, CultureInfo.InvariantCulture),
            Comment = f.GetComments(),
        };
        GetAttributes(f, res.Attributes);
        return res;
    }

    private void GetAttributes(ISymbol symbol, RepeatedField<AttributeRef> output)
    {
        symbol
            .GetAttributes()
            .Where(a => !IsIgnored(a.AttributeClass))
            .Select(ToAttribute)
            .Where(a => a is not null)
            .SaveToRepeatedField(output!);

        AttributeRef? ToAttribute(AttributeData a)
        {
            if (a.AttributeClass is null)
            {
                return null;
            }

            var type = a.AttributeClass.ToFullName();
            var positional = a
                .ConstructorArguments.SelectMany(FlattenPositionalArray)
                .Select(ToPositionalArgument)
                .Cast<AttributeArgument>();
            var named = a
                .NamedArguments.SelectMany(FlattenNamedArray)
                .Select(ToNamedArgument)
                .Cast<AttributeArgument>();
            var result = new AttributeRef { AttributeName = type };
            positional.Concat(named).SaveToRepeatedField(result.Argument);
            return result;

            static AttributeArgument ToNamedArgument((string Key, object? Value) v)
            {
                return new()
                {
                    Named = new() { Name = v.Key, Value = v.Value.ToValueRef() },
                };
            }

            static AttributeArgument ToPositionalArgument(object? v, int i)
            {
                return new()
                {
                    Positional = new() { Position = i, Value = v.ToValueRef() },
                };
            }

            static IEnumerable<object?> FlattenPositionalArray(TypedConstant a)
            {
                return a.Kind == TypedConstantKind.Array ? a.Values.Select(v => v.Value) : [a.Value];
            }

            static IEnumerable<(string Key, object? Value)> FlattenNamedArray(KeyValuePair<string, TypedConstant> a)
            {
                if (a.Value.Kind == TypedConstantKind.Array)
                {
                    return a.Value.Values.Select(v => (a.Key, v.Value));
                }
                else
                {
                    return [(a.Key, a.Value.Value)];
                }
            }
        }
    }
}
