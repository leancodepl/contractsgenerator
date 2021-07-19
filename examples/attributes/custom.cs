[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class CustomAttribute : System.Attribute
{
    public string NamedArg { get; set; }
}

[Custom(NamedArg = "Test")]
public class Dto { }
