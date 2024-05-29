using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit;

public class CSharpTests
{
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
}
