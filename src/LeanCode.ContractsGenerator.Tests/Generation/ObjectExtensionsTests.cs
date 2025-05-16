using LeanCode.ContractsGenerator.Generation;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.Generation;

public class ObjectExtensionsTests
{
    [Fact]
    public void Null_is_converted()
    {
        var vr = ((object?)null).ToValueRef();
        Assert.NotNull(vr.Null);
    }

    [Theory]
    [InlineData((byte)10)]
    [InlineData((sbyte)10)]
    [InlineData((short)10)]
    [InlineData((ushort)10)]
    [InlineData(10)]
    [InlineData(10L)]
    [InlineData(10U)]
    [InlineData(10UL)]
    public void Number_is_converted(object value)
    {
        var vr = value.ToValueRef();
        Assert.NotNull(vr.Number);
        Assert.Equal(10, vr.Number.Value);
    }

    [Theory]
    [InlineData(10.015000343322754f)]
    [InlineData(10.015000343322754d)]
    public void FloatingPoint_is_converted(object value)
    {
        var vr = value.ToValueRef();
        Assert.NotNull(vr.FloatingPoint);
        Assert.Equal(10.015000343322754, vr.FloatingPoint.Value);
    }

    [Fact]
    public void String_is_converted()
    {
        var vr = "abc".ToValueRef();
        Assert.NotNull(vr.String);
        Assert.Equal("abc", vr.String.Value);
    }

    [Fact]
    public void Bool_is_converted()
    {
        var vr = true.ToValueRef();
        Assert.NotNull(vr.Bool);
        Assert.True(vr.Bool.Value);
    }

    [Fact]
    public void Other_types_are_not_converted()
    {
        Assert.Throws<NotSupportedException>(() => new object().ToValueRef());
        Assert.Throws<NotSupportedException>(() => new Random().ToValueRef());
    }
}
