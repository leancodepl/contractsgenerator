namespace LeanCode.ContractsGenerator.Analyzers
{
    public class ErrorCodesUniqueness : BaseAnalyzer
    {
        public const string Code = "LNCD003";

        public override IEnumerable<AnalyzeError> AnalyzeErrorCodes(IEnumerable<ErrorCode> errorCodes)
        {
            return errorCodes
                .SelectMany(Flatten)
                .GroupBy(e => e.Code)
                .Where(g => g.Count() > 1)
                .Select(g => new AnalyzeError(Code, $"Duplicate error codes: {string.Join(", ", g.Select(c => c.Name))}", nameof(ErrorCode), ""));

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
            return export.Statements
                .SelectMany(AnalyzeStatement)
                .ToList();
        }

        public override IEnumerable<AnalyzeError> AnalyzeCommand(Statement stmt, Statement.Types.Command command)
        {
            return AnalyzeErrorCodes(command.ErrorCodes)
                .Select(e => e with { Name = stmt.Name });
        }

        public override IEnumerable<AnalyzeError> AnalyzeStatement(Statement stmt)
        {
            if (stmt.Command is Statement.Types.Command cmd)
            {
                return AnalyzeCommand(stmt, cmd);
            }
            else
            {
                return Enumerable.Empty<AnalyzeError>();
            }
        }
    }
}
