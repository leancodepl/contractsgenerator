using LeanCode.Contracts;

public class A : ICommand
{
    [System.Obsolete("Msg")]
    public string Prop { get; set; }
}
