using System.Collections.Immutable;

namespace LeanCode.ContractsGenerator.Analyzers;

public class ExternalTypeCheck : BaseAnalyzer
{
    private ImmutableHashSet<string> knownTypes = [];

    public override IEnumerable<AnalyzeError> Analyze(Export export)
    {
        knownTypes = GatherTypes(export);
        return base.Analyze(export);
    }

    public override IEnumerable<AnalyzeError> AnalyzeInternalTypeRef(
        AnalyzerContext context,
        TypeRef typeRef,
        TypeRef.Types.Internal i
    )
    {
        if (knownTypes.Contains(i.Name) || InvalidTypeCheck.InvalidTypes.ContainsKey(i.Name))
        {
            return base.AnalyzeInternalTypeRef(context, typeRef, i);
        }
        else
        {
            return
            [
                new AnalyzeError(
                    AnalyzerCodes.InternalTypeIsNotKnown,
                    $"Internal type `{i.Name}` is not known.",
                    context
                ),
            ];
        }
    }

    private static ImmutableHashSet<string> GatherTypes(Export export) => [.. export.Statements.Select(s => s.Name)];
}
