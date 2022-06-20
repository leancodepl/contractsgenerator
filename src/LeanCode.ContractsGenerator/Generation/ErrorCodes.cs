using System.Globalization;
using Microsoft.CodeAnalysis;

namespace LeanCode.ContractsGenerator.Generation;

public static class ErrorCodes
{
    private const string ErrorCodesName = "ErrorCodes";

    public static bool IsErrorCode(ISymbol? sym)
    {
        return
            sym?.Name == ErrorCodesName ||
            (sym?.ContainingSymbol is not null && IsErrorCode(sym.ContainingSymbol));
    }

    public static IEnumerable<ErrorCode> Extract(INamedTypeSymbol symbol)
    {
        var errCodes = symbol.GetMembers()
            .OfType<INamedTypeSymbol>()
            .Where(s => s.Name == ErrorCodesName)
            .SingleOrDefault();
        if (errCodes is not null)
        {
            return MapCodes(errCodes);
        }
        else
        {
            return Enumerable.Empty<ErrorCode>();
        }

        static IEnumerable<ErrorCode> MapCodes(INamedTypeSymbol errCodes)
        {
            var consts = errCodes
                .GetMembers()
                .OfType<IFieldSymbol>()
                .Select(ToSingleCode);
            var groups = errCodes
                .GetMembers()
                .OfType<INamedTypeSymbol>()
                .Select(ToGroupCode);
            return consts.Concat(groups);
        }

        static ErrorCode ToSingleCode(IFieldSymbol f)
        {
            if (!f.HasConstantValue)
            {
                throw new InvalidOperationException("The error codes class can only contain constant numeric fields & derived types.");
            }

            return new()
            {
                Single = new()
                {
                    Name = f.Name,
                    Code = Convert.ToInt32(f.ConstantValue, CultureInfo.InvariantCulture),
                },
            };
        }

        static ErrorCode ToGroupCode(INamedTypeSymbol ns)
        {
            if (ns.BaseType?.Name != ErrorCodesName)
            {
                throw new InvalidOperationException($"The base class for error codes group needs to be named `{ErrorCodesName}`.");
            }

            var g = new ErrorCode.Types.Group
            {
                Name = ns.Name,
                GroupId = ns.BaseType.ToFullName(),
            };
            MapCodes(ns.BaseType).SaveToRepeatedField(g.InnerCodes);
            return new() { Group = g };
        }
    }

    public static IEnumerable<ErrorCode.Types.Group> ListKnownGroups(IEnumerable<Statement> statements)
    {
        return statements
            .Where(s => s.Command is not null)
            .SelectMany(c => ListGroups(c.Command.ErrorCodes));

        static IEnumerable<ErrorCode.Types.Group> ListGroups(IEnumerable<ErrorCode> gs)
        {
            foreach (var g in gs.Where(i => i.Group is not null))
            {
                yield return g.Group;

                foreach (var i in ListGroups(g.Group.InnerCodes))
                {
                    yield return i;
                }
            }
        }
    }
}
