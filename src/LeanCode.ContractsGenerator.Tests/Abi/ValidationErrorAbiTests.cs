using LeanCode.Contracts.Validation;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.Abi;

public class ValidationErrorAbiTests
{
    [Fact]
    public void ValidationError_has_3_params_ctor()
    {
        Assert.NotNull(typeof(ValidationError).GetConstructor([typeof(string), typeof(string), typeof(int)]));
    }

    [Fact]
    public void ValidationError_has_4_params_ctor()
    {
        Assert.NotNull(
            typeof(ValidationError).GetConstructor([typeof(string), typeof(string), typeof(int), typeof(string)])
        );
    }
}
