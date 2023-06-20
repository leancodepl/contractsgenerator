using LeanCode.Contracts;

public class EmptyTopic : ITopic { }
public class EmptyInheritedTopic : EmptyTopic { }

public interface IEmptyTopic : ITopic { } // this is fine but the one below is not
public class InheritedInterfaceEmptyTopic : IEmptyTopic { }

public abstract class AbstractEmptyTopic : ITopic { } // this is fine but the one below is not
public class ConcreteEmptyTopic : AbstractEmptyTopic { }
