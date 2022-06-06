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
    public Date H { get; set; }
    public Time I { get; set; }
    public DateTime J { get; set; }
    public DateTimeOffset K { get; set; }
    public Guid L { get; set; }
    public float M { get; set; }
    public double N { get; set; }
    public TimeSpan O { get; set; }
    public CommandResult P { get; set; }
}
