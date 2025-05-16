namespace LeanCode.ContractsGenerator.Analyzers;

public class ErrorCodesUniqueness : BaseAnalyzer
{
    public override IEnumerable<AnalyzeError> AnalyzeErrorCodes(
        AnalyzerContext context,
        IEnumerable<ErrorCode> errCodes
    )
    {
        return errCodes
            .SelectMany(Flatten)
            .GroupBy(e => e.Code)
            .Where(g => g.Count() > 1)
            .Select(g => new AnalyzeError(
                AnalyzerCodes.DuplicateErrorCodes,
                $"Duplicate error codes: {string.Join(", ", g.Select(c => c.Name))}",
                context
            ));

        static IEnumerable<ErrorCode.Types.Single> Flatten(ErrorCode errCode)
        {
            return errCode.Single is ErrorCode.Types.Single s ? [s] : errCode.Group.InnerCodes.SelectMany(Flatten);
        }
    }

    public override IEnumerable<AnalyzeError> Analyze(Export export)
    {
        var context = AnalyzerContext.Empty;

        return [.. export.Statements.SelectMany(s => AnalyzeStatement(context.Descend(s), s))];
    }

    public override IEnumerable<AnalyzeError> AnalyzeCommand(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Command command
    ) => AnalyzeErrorCodes(context.ErrorCodes(), command.ErrorCodes);

    public override IEnumerable<AnalyzeError> AnalyzeStatement(AnalyzerContext context, Statement stmt) =>
        stmt.Command is Statement.Types.Command cmd ? AnalyzeCommand(context, stmt, cmd) : [];
}
