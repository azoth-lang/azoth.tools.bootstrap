namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework;

/// <summary>
/// This test class shows that there is no good way to call a protected instance method of an interface.
/// </summary>
/// <remarks>The C# lang team has designed a feature for this, but it doesn't seem to be going
/// anywhere. <see href="https://github.com/dotnet/csharplang/issues/2337">Champion: base(T) phase two #2337</see></remarks>
public class ProtectedInterfaceMethodTests
{

    public interface ITest
    {
        protected sealed int TestMethod() => 42;
    }

    public class Test : ITest
    {
        public int Something()
            => Helper(this);
        // Proposed syntax: base(ITest).TestMethod();

        private static int Helper<T>(T self)
            where T : Test, ITest
        {
            return self.TestMethod(); // this works
        }
    }
}
