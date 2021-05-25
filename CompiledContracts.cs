using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LeanCode.ContractsGeneratorV2
{
    public sealed class CompiledContracts
    {
        private readonly CSharpCompilation compilation;
        private readonly IReadOnlyList<SyntaxTree> trees;

        public ContractTypes Types { get; }

        public CompiledContracts(CSharpCompilation compilation, List<SyntaxTree> trees)
        {
            this.compilation = compilation;
            this.trees = trees;

            Types = new(compilation);
        }

        public IEnumerable<INamedTypeSymbol> ListAllTypes()
        {
            return trees.SelectMany(t =>
            {
                var model = compilation.GetSemanticModel(t);
                var root = t.GetRoot();
                var symbols = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
                return symbols
                    .Select(s => model.GetDeclaredSymbol(s))
                    .OfType<INamedTypeSymbol>();
            });
        }
    }
}
