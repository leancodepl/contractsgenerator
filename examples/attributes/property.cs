using LeanCode.CQRS;

public class A : IRemoteCommand
{
    [System.Obsolete("Msg")]
    public string Prop { get; set; }
}
