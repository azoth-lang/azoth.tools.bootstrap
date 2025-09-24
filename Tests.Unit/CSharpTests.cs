using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit;

public class CSharpTests
{
    [Fact]
    public void LogicalOperatorsOnNullableBooleans()
    {
        bool? n1 = null;
        bool? n2 = null;
        bool? t = true;
        bool? f = false;

        Assert.True(n1 == n2);
        Assert.Null(n1 & t);
        Assert.False(n1 & f);
        Assert.Null(n1 & n2);
        // Operator not allowed
        //Assert.True(n && t);
    }

    [Fact]
    public void ComparisonOnNullableInts()
    {
        int? n1 = null;
        int? n2 = null;
        int? one = 1;

        Assert.True(n1 == n2);
        Assert.False(n1 <= n2);
        Assert.False(n1 < one);
        Assert.False(one < n1);
    }

    /// <summary>
    /// This test was meant to see whether exposing <c>Child&lt;T></c> properties could be done
    /// and have the <c>.Value</c> not be required.
    /// </summary>
    [Fact]
    public void GenericImplicitConversionToInterfaceNotSupported()
    {
        var box = new Box<IFoo>(null!);

        IFoo value1 = box.Value;
        // The line below does not compile because the implicit operator is not found
        //IFoo value2 = box;

        Assert.Null(value1);
    }

    /// <summary>
    /// This should that even if you made an actual class, the implicit conversion would not kick in
    /// when assigning to an interface.
    /// </summary>
    [Fact]
    public void GenericImplicitConversionToClassNotInferred()
    {
        var box = new Box<Foo>(new());

        IFoo value1 = box.Value;
        // The line below does not compile because the implicit operator is not found
        //IFoo value2 = box;

        Assert.Same(box.Value, value1);
    }

    #region Types for Generic Implicit Conversion Tests
    private sealed class Box<T>
        where T : class?
    {
        public static implicit operator T(Box<T> value) => value.Value;

        public readonly T Value;

        public Box(T value)
        {
            Value = value;
        }
    }

    private interface IFoo
    {
    }

    private class Foo : IFoo
    {
    }
    #endregion

    #region Single Implementation
    private interface IBase
    {
        int Method();
    }
    private interface IBase<T> : IBase
    {
        void Procedure(T value);
    }

    private interface IOne : IBase<int>
    {
        int IBase.Method() => Method();
        new int Method() => 1;

        void IBase<int>.Procedure(int value) => Procedure(value);
        new void Procedure(int value) { }
    }

    private interface ITwo : IBase<string>
    {
        int IBase.Method() => Method();
        new int Method() => 2;

        void IBase<string>.Procedure(string value) => Procedure(value);
        new void Procedure(string value) { }
    }

    private class Class : IOne, ITwo
    {
        // C# gives an error about not having a most specific method implementation if this method is
        // missing. Azoth needs to either require the same, or deal with how to upcast Class to IBase
        // given that there are two different implementations of IBase
        public int Method() => throw new System.NotImplementedException();
    }
    #endregion

    //[Fact]
    //public void AbstractStatic()
    //{
    //    var b = new Box<IAbstractStaticTest>(new AbstractStaticTest());
    //}

    //private interface IAbstractStaticTest
    //{
    //    public static abstract void Test();
    //}

    //private class AbstractStaticTest : IAbstractStaticTest
    //{
    //    public static void Test() => throw new System.NotImplementedException();
    //}
}
