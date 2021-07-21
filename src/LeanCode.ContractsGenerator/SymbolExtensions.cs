using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator
{
    public static class SymbolExtensions
    {
        public static string ToFullName(this ISymbol symbol)
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

        public static string GetComments(this ISymbol symbol)
        {
            var xml = symbol.GetDocumentationCommentXml();
            if (!string.IsNullOrEmpty(xml))
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                if (doc.DocumentElement is not null)
                {
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
            }
            else
            {
                return string.Empty;
            }

            static IEnumerable<XmlNode> FlattenAllNodes(XmlNode n)
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
