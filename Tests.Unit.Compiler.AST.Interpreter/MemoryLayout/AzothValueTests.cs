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
}
