using LeanCode.Contracts;
using LeanCode.DomainModels.Model;

namespace PackageReference;

[ExcludeFromContractsGeneration]
public class Order : IIdentifiable<LId<Order>>
{
    public LId<Order> Id { get; set; }
}

public class OrderDTO
{
    private LId<Order> id; // Roslyn won't compile this if packages are not restored

    public long Id
    {
        get => id.Value;
        set => id = new(value);
    }
}
