namespace LeanCode.ContractsGenerator.Analyzers;

public class ErrorCodesUniqueness : BaseAnalyzer
{
    public const string Code = "CNTR0003";

    public override IEnumerable<AnalyzeError> AnalyzeErrorCodes(AnalyzerContext context, IEnumerable<ErrorCode> errCodes)
    {
        return errCodes
            .SelectMany(Flatten)
            .GroupBy(e => e.Code)
            .Where(g => g.Count() > 1)
            .Select(g => new AnalyzeError(Code, $"Duplicate error codes: {string.Join(", ", g.Select(c => c.Name))}", context));

        static IEnumerable<ErrorCode.Types.Single> Flatten(ErrorCode errCode)
        {
            if (errCode.Single is ErrorCode.Types.Single s)
            {
                return new[] { s };
            }
            else
            {
                return errCode.Group.InnerCodes.SelectMany(Flatten);
            }
        }
    }

    public override IEnumerable<AnalyzeError> Analyze(Export export)
    {
        var context = AnalyzerContext.Empty;

        return export.Statements
            .SelectMany(s => AnalyzeStatement(context.Descend(s), s))
            .ToList();
    }

    public override IEnumerable<AnalyzeError> AnalyzeCommand(AnalyzerContext context, Statement stmt, Statement.Types.Command command)
    {
        return AnalyzeErrorCodes(context.ErrorCodes(), command.ErrorCodes);
    }

    public override IEnumerable<AnalyzeError> AnalyzeStatement(AnalyzerContext context, Statement stmt)
    {
        if (stmt.Command is Statement.Types.Command cmd)
        {
            return AnalyzeCommand(context, stmt, cmd);
        }
        else
        {
            return Enumerable.Empty<AnalyzeError>();
        }
    }
}
