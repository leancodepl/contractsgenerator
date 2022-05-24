using Google.Protobuf.Collections;
using LeanCode.ContractsGenerator.Compilation;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator.Generation;

public class ContractsGenerator
{
    private readonly CompiledContracts contracts;

    private readonly TypeRefFactory typeRef;

    public ContractsGenerator(CompiledContracts contracts)
    {
        this.contracts = contracts;

        typeRef = new(contracts);
    }

    public Export Generate()
    {
        var export = GenerateCore();
        return Analyze(export);
    }

    public Export Generate(List<Export> externalContracts)
    {
        var export = GenerateCore();
        var name = export.ProjectName; // preserve name to restore it later after MergeFrom overwrites it

        foreach (var ee in externalContracts)
        {
            export.MergeFrom(ee);
        }

        export.ProjectName = name;

        return Analyze(export);
    }

    private Export GenerateCore()
    {
        var export = new Export() { ProjectName = contracts.ProjectName };
        contracts.ListAllTypes()
            .Select(ProcessType)
            .Where(s => s is not null)
            .Cast<Statement>()
            .ToList()
            .OrderBy(s => s.Name)
            .SaveToRepeatedField(export.Statements);
        ErrorCodes.ListKnownGroups(export.Statements)
            .SaveToRepeatedField(export.KnownErrorGroups);
        return export;
    }

    private static Export Analyze(Export export)
    {
        var errors = new Analyzers.AllAnalyzers().Analyze(export).ToList();
        if (errors.Count > 0)
        {
            throw new AnalyzeFailedException(errors);
        }
        else
        {
            return export;
        }
    }

    private Statement? ProcessType(INamedTypeSymbol? symbol)
    {
        if (IsIgnored(symbol) || IsExcluded(symbol))
        {
            return null;
        }

        var result = new Statement
        {
            Name = symbol.ToFullName(),
            Comment = symbol.GetComments(),
        };
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
                result.Command = new()
                {
                    TypeDescriptor = MapTypeDescriptor(symbol),
                };
                ErrorCodes.Extract(symbol).SaveToRepeatedField(result.Command.ErrorCodes);
            }
            else if (symbol.TypeKind == TypeKind.Enum)
            {
                result.Enum = new();
                symbol.GetMembers()
                    .OfType<IFieldSymbol>()
                    .Select(ToEnumValue)
                    .SaveToRepeatedField(result.Enum.Members);
            }
            else
            {
                result.Dto = new()
                {
                    TypeDescriptor = MapTypeDescriptor(symbol),
                };
            }
        }
    }

    private TypeDescriptor MapTypeDescriptor(INamedTypeSymbol symbol)
    {
        var descriptor = new TypeDescriptor();
        symbol.TypeParameters
            .Select(ToParam)
            .SaveToRepeatedField(descriptor.GenericParameters);
        symbol.Interfaces
            .Append(symbol.BaseType)
            .Where(IsNotIgnored)
            .Select(typeRef.From!)
            .SaveToRepeatedField(descriptor.Extends);
        MapProperties(symbol, descriptor);
        symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(fs => fs.HasConstantValue)
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

    private bool IsNotIgnored([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] INamedTypeSymbol? symbol)
    {
        return !IsIgnored(symbol);
    }

    private bool IsIgnored([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] INamedTypeSymbol? symbol)
    {
        return symbol is null ||
            symbol.SpecialType == SpecialType.System_Object ||
            symbol.SpecialType == SpecialType.System_Enum ||
            ErrorCodes.IsErrorCode(symbol) ||
            contracts.Types.IsAttributeUsageType(symbol);
    }

    private bool IsExcluded(ISymbol symbol)
    {
        return symbol
            .GetAttributes()
            .Any(a => contracts.Types.IsExcludeFromContractsGenerationType(a.AttributeClass));
    }

    private static HashSet<string> GatherBaseProperties(INamedTypeSymbol ns)
    {
        return ns.AllInterfaces
            .SelectMany(i => i.GetMembers())
            .OfType<IPropertySymbol>()
            .Select(p => p.Name)
            .ToHashSet();
    }

    private static bool AlreadyImplemented(IPropertySymbol prop, HashSet<string> baseProps)
    {
        return baseProps.Contains(prop.Name);
    }

    private GenericParameter ToParam(ITypeParameterSymbol ts)
    {
        return new() { Name = ts.Name };
    }

    private ConstantRef ToConstant(IFieldSymbol fs)
    {
        return new()
        {
            Name = fs.Name,
            Value = fs.ConstantValue.ToValueRef(),
            Comment = fs.GetComments(),
        };
    }

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

    private static EnumValue ToEnumValue(IFieldSymbol f)
    {
        return new EnumValue()
        {
            Name = f.Name,
            Value = Convert.ToInt64(f.ConstantValue),
            Comment = f.GetComments(),
        };
    }

    private void GetAttributes(ISymbol symbol, RepeatedField<AttributeRef> output)
    {
        symbol.GetAttributes()
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
            var positional = a.ConstructorArguments
                .SelectMany(FlattenPositionalArray)
                .Select(ToPositionalArgument)
                .Cast<AttributeArgument>();
            var named = a.NamedArguments
                .SelectMany(FlattenNamedArray)
                .Select(ToNamedArgument)
                .Cast<AttributeArgument>();
            var result = new AttributeRef { AttributeName = type };
            positional.Concat(named).SaveToRepeatedField(result.Argument);
            return result;

            static AttributeArgument ToNamedArgument((string Key, object? Value) v)
            {
                return new()
                {
                    Named = new()
                    {
                        Name = v.Key,
                        Value = v.Value.ToValueRef(),
                    },
                };
            }

            static AttributeArgument ToPositionalArgument(object? v, int i)
            {
                return new()
                {
                    Positional = new()
                    {
                        Position = i,
                        Value = v.ToValueRef(),
                    },
                };
            }

            static IEnumerable<object?> FlattenPositionalArray(TypedConstant a)
            {
                return a.Kind == TypedConstantKind.Array ? a.Values.Select(v => v.Value) : new[] { a.Value };
            }

            static IEnumerable<(string Key, object? Value)> FlattenNamedArray(KeyValuePair<string, TypedConstant> a)
            {
                if (a.Value.Kind == TypedConstantKind.Array)
                {
                    return a.Value.Values.Select(v => (a.Key, v.Value));
                }
                else
                {
                    return new[] { (a.Key, a.Value.Value) };
                }
            }
        }
    }
}
