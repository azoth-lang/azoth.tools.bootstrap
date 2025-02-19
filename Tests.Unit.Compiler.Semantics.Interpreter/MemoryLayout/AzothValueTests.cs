using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Semantics.Interpreter.MemoryLayout;

public class AzothValueTests
{
    [Fact]
    public unsafe void SizeOfAzothValue()
    {
        var size = Unsafe.SizeOf<AzothValue>();

        var expectedSize = sizeof(IntPtr) + sizeof(long);
        Assert.Equal(expectedSize, size);
    }

    [Fact]
    public void NoneIsNone()
    {
        Assert.True(AzothValue.None.IsNone);
    }

    /// <summary>
    /// Due to the memory layout of <see cref="AzothValue"/> and <see cref="BigInteger"/> a special
    /// flag reference must be used to indicate `none`. If that isn't done, then the `IsNone`
    /// property could incorrectly report that an integer value is `none`.
    /// </summary>
    [Fact]
    public void IntIsNotNone()
    {
        Assert.False(AzothValue.Int(42).IsNone);
    }
}
