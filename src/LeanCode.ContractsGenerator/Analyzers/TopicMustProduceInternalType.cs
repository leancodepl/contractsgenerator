namespace LeanCode.ContractsGenerator.Analyzers;

public class TopicMustProduceInternalType : BaseAnalyzer
{
    public override IEnumerable<AnalyzeError> AnalyzeTopic(
        AnalyzerContext context,
        Statement stmt,
        Statement.Types.Topic topic
    )
    {
        foreach (var notification in topic.Notifications)
        {
            if (notification.Type.Internal is null)
            {
                yield return new AnalyzeError(
                    AnalyzerCodes.TopicMustProduceInternalType,
                    $"Topic `{stmt.Name}` produces `{notification.Type}` which is not internal type. Topics must produce internal types only.",
                    context
                );
            }
        }
    }
}
