using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.AST.Interpreter.MemoryLayout;

public class AzothValueTests
{
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
