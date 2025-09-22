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
