namespace LeanCode.ContractsGenerator.Analyzers;

public class TopicProducesNotificationCheck : BaseAnalyzer
{
    public const string Code = "CNTR0007";

    public override IEnumerable<AnalyzeError> AnalyzeTopic(AnalyzerContext context, Statement stmt, Statement.Types.Topic topic)
    {
        var errors = base.AnalyzeTopic(context, stmt, topic);
        if (topic.Notifications.Count == 0)
        {
            errors = errors.Append(
                new AnalyzeError(Code, $"Topic type `{stmt.Name}` doesn't produce any notification.", context));
        }

        return errors;
    }
}
