namespace LeanCode.ContractsGenerator.Analyzers;

public class TopicProducesNotificationCheck : BaseAnalyzer
{
    public const string NoNotificationCode = "CNTR0007";
    public const string NullableNotificationCode = "CNTR0008";

    public override IEnumerable<AnalyzeError> AnalyzeTopic(AnalyzerContext context, Statement stmt, Statement.Types.Topic topic)
    {
        if (topic.Notifications.Count == 0)
        {
            yield return new AnalyzeError(NoNotificationCode, $"Topic type `{stmt.Name}` doesn't produce any notification.", context);
        }

        var nullableNotifications = topic.Notifications.Where(n => n.Nullable);

        foreach (var notification in nullableNotifications)
        {
            yield return new AnalyzeError(
                NullableNotificationCode,
                $"Topic type `{stmt.Name}` produces nullable notification type `{notification.Internal.Name}`.",
                context);
        }
    }
}
