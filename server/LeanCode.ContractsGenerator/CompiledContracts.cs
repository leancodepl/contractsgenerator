using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LeanCode.ContractsGenerator
{
    public sealed class CompiledContracts
    {
        private readonly CSharpCompilation compilation;

        public ContractTypes Types { get; }
        public string ProjectName { get; }

        public CompiledContracts(CSharpCompilation compilation, string projectName)
        {
            this.compilation = compilation;
            ProjectName = projectName;

            Types = new(compilation);
        }

        public IEnumerable<INamedTypeSymbol> ListAllTypes()
        {
            return compilation.SyntaxTrees.SelectMany(t =>
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
