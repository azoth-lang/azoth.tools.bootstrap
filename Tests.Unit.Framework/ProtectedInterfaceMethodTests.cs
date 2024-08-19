using InlineMethod;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework;

/// <summary>
/// This test class shows that there is no good way to call a protected instance method of an interface.
/// </summary>
/// <remarks>The C# lang team has designed a feature for this, but it doesn't seem to be going
/// anywhere. <see href="https://github.com/dotnet/csharplang/issues/2337">Champion: base(T) phase two #2337</see></remarks>
public class ProtectedInterfaceMethodTests
{
    [Fact]
    public void CallProtectedInterfaceMethod()
    {
        var test = new Test();

        var result = test.Something();

        Assert.Equal(42, result);
    }

    public interface ITest
    {
        protected sealed int TestMethod() => 42;
    }

    public class Test : ITest
    {
        public int Something()
           //=> TestMethod(); // doesn't work
           //=> ((ITest)this).TestMethod(); // doesn't work
           => Helper(this);
        // Proposed syntax: base(ITest).TestMethod();

        [Inline] // Inline does work though
        private static int Helper<T>(T self)
            where T : Test, ITest
            => self.TestMethod(); // this works
    }
}
