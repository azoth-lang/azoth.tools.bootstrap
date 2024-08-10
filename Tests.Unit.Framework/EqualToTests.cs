using System;
using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework;

public class EqualToTests
{
    [Fact]
    public void Constraint_satisfied_by_base_equality()
    {
        Parent test = new Child(42);
        UseEqualTo(test);
        //UseEquatable(test);
    }


    //[Fact]
    //public void Nullable_equality()
    //{
    //    int? test = 42;

    //    UseEquatable(test);
    //}

    [Fact]
    public void Primitive()
    {
        int test = 42;

        //UseEqualTo(test);
        UseEquatable(test);
    }

    private void UseEqualTo<T>(T value) where T : IEqualTo<T> { }

    private void UseEquatable<T>(T value) where T : IEquatable<T> { }

    private class Parent : IEqualTo<Parent>
    {
        public int Value { get; }

        public Parent(int value)
        {
            Value = value;
        }

        protected virtual bool CanEqual(Parent other) => true;
        bool IEqualTo<Parent>.CanEqual(Parent other) => CanEqual(other);

        public bool EqualTo(Parent other) => ReferenceEquals(this, other);
    }

    private sealed class Child : Parent
    {
        public Child(int value) : base(value) { }


        protected override bool CanEqual(Parent other) => other is Child;

        public bool EqualTo(Child other) => ReferenceEquals(this, other);
    }
}
