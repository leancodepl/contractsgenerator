using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator.Generation;

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "?",
        "CA1031",
        Justification = "Exception boundary."
    )]
    public static string GetComments(this ISymbol symbol)
    {
        var xml = symbol.GetDocumentationCommentXml();
        if (!string.IsNullOrEmpty(xml))
        {
            try
            {
                return ExtractFromXml(xml);
            }
            catch
            {
                return string.Empty;
            }
        }
        else
        {
            return string.Empty;
        }

        static string ExtractFromXml(string xml)
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

                return sb.ToString().TrimEnd();
            }
            else
            {
                return string.Empty;
            }
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
