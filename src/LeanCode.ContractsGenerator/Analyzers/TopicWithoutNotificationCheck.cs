namespace LeanCode.ContractsGenerator.Analyzers;

public class TopicWithoutNotificationCheck : BaseAnalyzer
{
    public const string Code = "CNTR0007";

    public override IEnumerable<AnalyzeError> AnalyzeTopic(AnalyzerContext context, Statement stmt, Statement.Types.Topic topic)
    {
        if (topic.Notifications.Count == 0)
        {
            yield return new AnalyzeError(Code, $"Topic type `{stmt.Name}` doesn't produce any notification.", context);
        }
    }
}
