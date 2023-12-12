using LeanCode.Contracts;

public record DTO1(int Property);

public record DTO2(string Property);

public class Command : ICommand
{
    public DTO1 DTO1 { get; set; }
}

public class Query : IQuery<DTO2>
{
    public DTO1 DTO1 { get; set; }
}

public class Operation : IOperation<DTO2>
{
    public DTO1 DTO1 { get; set; }
}
