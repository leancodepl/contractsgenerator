namespace LeanCode.ContractsGenerator.Analyzers;

public class TopicWithoutNotificationCheck : BaseAnalyzer
{
    public override IEnumerable<AnalyzeError> AnalyzeTopic(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Topic topic
    )
    {
        if (topic.Notifications.Count == 0)
        {
            yield return new AnalyzeError(
                AnalyzerCodes.TopicDoesNotProduceNotification,
                $"Topic type `{stmt.Name}` doesn't produce any notification.",
                context
            );
        }
    }
}
