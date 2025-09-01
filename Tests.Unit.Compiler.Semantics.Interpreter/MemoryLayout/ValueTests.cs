using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Semantics.Interpreter.MemoryLayout;

public class ValueTests
{
    [Fact]
    public unsafe void SizeOfAzothValue()
    {
        var size = Unsafe.SizeOf<Value>();

        var expectedSize = sizeof(IntPtr) + sizeof(long);
        Assert.Equal(expectedSize, size);
    }

    [Fact]
    public void NoneIsNone()
    {
        Assert.True(Value.None.IsNone);
    }

    /// <summary>
    /// Due to the memory layout of <see cref="Value"/> and <see cref="BigInteger"/> a special
    /// flag reference must be used to indicate `none`. If that isn't done, then the `IsNone`
    /// property could incorrectly report that an integer value is `none`.
    /// </summary>
    [Fact]
    public void IntIsNotNone()
    {
        Assert.False(Value.From(42).IsNone);
    }
}
