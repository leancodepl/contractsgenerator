using System;
public class Dto
{
    public int A { get; set; }
    public ulong B { get; }
    public string C { get; set; }
    public bool D { get; private set; }
    public Uri E { get; set; }
    public Date F { get; set; }
    public Time G { get; set; }
    public DateTime H { get; set; }
    public DateTimeOffset I { get; set; }
    public Guid J { get; set; }
    public float K { get; set; }
    public double L { get; set; }
    public TimeSpan M { get; set; }
}
