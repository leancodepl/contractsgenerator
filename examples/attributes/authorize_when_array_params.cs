using LeanCode.Contracts;
using LeanCode.Contracts.Security;

[AuthorizeWhenHasAnyOf(new[] { "P1", "P2" })]
public class A : ICommand { }

[AuthorizeWhenHasAnyOf("P1", "P2")]
public class B : ICommand { }
