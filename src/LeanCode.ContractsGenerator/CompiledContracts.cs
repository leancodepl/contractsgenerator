using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LeanCode.ContractsGenerator
{
    public sealed class CompiledContracts
    {
        private readonly IReadOnlyList<CSharpCompilation> compilations;

        public ContractTypes Types { get; }
        public string ProjectName { get; }

        public CompiledContracts(IReadOnlyList<CSharpCompilation> compilations, string projectName)
        {
            this.compilations = compilations;
            ProjectName = projectName;

            Types = new(compilations);
        }

        public IEnumerable<INamedTypeSymbol> ListAllTypes()
        {
            return compilations
                .SelectMany(c => c.SyntaxTrees
                    .SelectMany(t =>
                    {
                        var model = c.GetSemanticModel(t);
                        var root = t.GetRoot();
                        var symbols = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
                        return symbols
                            .Select(s => model.GetDeclaredSymbol(s))
                            .OfType<INamedTypeSymbol>();
                    }));
        }
    }
}
