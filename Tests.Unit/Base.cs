namespace Azoth.Tools.Bootstrap.Tests.Unit;

public interface IInterface
{
    void InterfaceMethod() { }
}

public abstract class Base<TBase> : IInterface
{
    public virtual TBase BaseMethod(TBase v) => v;

    public virtual TBase OverrideMethod(TBase v) => v;

    public virtual IInterface CovariantMethod() => this;
}


public class Derived : Base<int>
{
    public override int OverrideMethod(int v) => v;

    public override Derived CovariantMethod() => this;

    public virtual int DerivedMethod(int v) => v;
}


public static class Call
{
    /// <summary>
    /// This test looks at the IL generated for method calls.
    /// </summary>
    /// <remarks>
    /// The IL shows that the CLR almost always expects one to call the base most method. For example,
    /// when calling OverrideMethod it does
    /// <c>callvirt instance !0 class Azoth.Tools.Bootstrap.Tests.Unit.Base`1&lt;int32>::OverrideMethod(!0)</c>.
    /// In Azoth, it seems like renaming and changing of parameter types means that it needs to call
    /// the most derived version of the method. This does seem to be born out by the covariant
    /// return method which even in C# calls the more derived version of the method.
    /// </remarks>
    public static void Test()
    {
        var c = new Derived();

        var i = (IInterface)c;
        i.InterfaceMethod();

        _ = c.BaseMethod(42);

        _ = c.OverrideMethod(34);

        c = c.CovariantMethod();

        _ = c.DerivedMethod(46);
    }
}
