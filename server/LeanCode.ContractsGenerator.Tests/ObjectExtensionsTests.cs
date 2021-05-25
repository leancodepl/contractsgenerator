using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void Null_is_converted()
        {
            var vr = ((object?)null).ToValueRef();
            Assert.NotNull(vr.Null);
        }

#pragma warning disable SA1139
        [Theory]
        [InlineData((byte)10)]
        [InlineData((sbyte)10)]
        [InlineData((int)10)]
        [InlineData((long)10)]
        [InlineData((short)10)]
        [InlineData((uint)10)]
        [InlineData((ulong)10)]
        [InlineData((ushort)10)]
        public void Number_is_converted(object value)
        {
            var vr = value.ToValueRef();
            Assert.NotNull(vr.Number);
            Assert.Equal(10, vr.Number.Value);
        }

        [Theory]
        [InlineData((float)10.015000343322754)]
        [InlineData((double)10.015000343322754)]
        public void FloatingPoint_is_converted(object value)
        {
            var vr = value.ToValueRef();
            Assert.NotNull(vr.FloatingPoint);
            Assert.Equal(10.015000343322754, vr.FloatingPoint.Value);
        }
#pragma warning restore

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
    }
}
