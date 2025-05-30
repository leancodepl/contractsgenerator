using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LeanCode.ContractsGenerator.Compilation;

public sealed class CompiledContracts(IReadOnlyCollection<CSharpCompilation> compilations, string projectName)
{
    public ContractTypes Types { get; } = new(compilations);
    public string ProjectName { get; } = projectName;

    public IEnumerable<INamedTypeSymbol> ListAllTypes() =>
        compilations.SelectMany(c =>
            c.SyntaxTrees.SelectMany(t =>
            {
                var model = c.GetSemanticModel(t);
                var root = t.GetRoot();
                var symbols = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
                return symbols
                    .Select(s => model.GetDeclaredSymbol(s))
                    .Where(s => s is not null)
                    .OfType<INamedTypeSymbol>();
            })
        );
}
