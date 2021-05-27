using LeanCode.CQRS;

namespace FirstNamespace
{
    public class DTO { }

    public class Command : IRemoteCommand { }

    public class Query : IRemoteQuery<int> { }
}

namespace SecondNamespace
{
    public class DTO { }

    public class Command : IRemoteCommand { }

    public class Query : IRemoteQuery<int> { }
}
