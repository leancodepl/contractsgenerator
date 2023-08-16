namespace LeanCode.ContractsGenerator.Analyzers;

public class TopicWithNullableNotificationCheck : BaseAnalyzer
{
    public override IEnumerable<AnalyzeError> AnalyzeTopic(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Topic topic
    )
    {
        var nullableNotifications = topic.Notifications.Where(n => n.Type.Nullable);

        foreach (var notification in nullableNotifications)
        {
            yield return new AnalyzeError(
                AnalyzerCodes.TopicProducesNullableNotification,
                $"Topic type `{stmt.Name}` produces nullable notification type.",
                context
            );
        }
    }
}
