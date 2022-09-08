using System;
using LeanCode.Contracts;

public class Dto
{
    public int A { get; set; }
    public ulong B { get; }
    public string C { get; set; }
    public bool D { get; private set; }
    public Uri E { get; set; }
    public DateOnly F { get; set; }
    public TimeOnly G { get; set; }
    public DateTimeOffset H { get; set; }
    public Guid I { get; set; }
    public float J { get; set; }
    public double K { get; set; }
    public TimeSpan L { get; set; }
    public CommandResult M { get; set; }
}
