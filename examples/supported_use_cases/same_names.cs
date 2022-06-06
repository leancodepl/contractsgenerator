using LeanCode.Contracts;

namespace First
{
    public class DTO { }

    public class Command : ICommand { }

    public class Query : IQuery<int> { }
}

namespace Second
{
    public class DTO { }

    public class Command : ICommand { }

    public class Query : IQuery<int> { }
}
