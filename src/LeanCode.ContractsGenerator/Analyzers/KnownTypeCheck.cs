namespace LeanCode.ContractsGenerator.Analyzers
{
    public class KnownTypeCheck : BaseAnalyzer
    {
        private static readonly IReadOnlySet<KnownType> ValidKnownTypeValues = Enum.GetValues<KnownType>().ToHashSet();

        public const string Code = "LNCD002";

        public override IEnumerable<AnalyzeError> AnalyzeKnownType(KnownType knownType)
        {
            if (!ValidKnownTypeValues.Contains(knownType))
            {
                yield return new(Code, $"`KnownType` value {knownType} is unsupported.", knownType.ToString(), knownType.ToString());
            }
        }
    }
}
