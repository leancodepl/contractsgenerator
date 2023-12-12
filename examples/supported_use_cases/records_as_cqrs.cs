using LeanCode.Contracts;

public record DTO1(int Property);

public record DTO2(string Property);

public record Command(DTO1 DTO1) : ICommand;

public record Query(DTO1 DTO1) : IQuery<DTO2>;

public record Operation(DTO1 DTO1) : IOperation<DTO2>;
