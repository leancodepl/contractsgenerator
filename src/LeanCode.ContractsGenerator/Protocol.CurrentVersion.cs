namespace LeanCode.ContractsGenerator;

public partial class Protocol
{
    public static readonly ProtocolVersion CurrentVersion = new(1);
    public static readonly string CurrentVersionAsString = CurrentVersion.ToString();

    public static class KnownExtensions
    {
        public const string DateTime = "datetime";
    }
}
