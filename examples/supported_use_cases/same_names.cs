using LeanCode.CQRS;

namespace First
{
    public class DTO { }

    public class Command : IRemoteCommand { }

    public class Query : IRemoteQuery<int> { }
}

namespace Second
{
    public class DTO { }

    public class Command : IRemoteCommand { }

    public class Query : IRemoteQuery<int> { }
}
