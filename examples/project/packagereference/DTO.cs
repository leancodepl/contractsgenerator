using Dapper;
using LeanCode.Contracts;

namespace PackageReference;

[ExcludeFromContractsGeneration]
public class FakeDTO
{
    public DbString Id { get; set; }
}

public class OrderDTO
{
    private DbString id; // Roslyn won't compile this if packages are not restored

    public string Id
    {
        get => id.Value;
        set => id = new() { Value = value };
    }
}
