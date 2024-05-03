using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public struct Child<T>
    where T : class
{
    private T? initialValue;

    public Child(T initialValue)
    {
        this.initialValue = initialValue;
    }

    public T Value => initialValue!;
}

public static class Child
{
    public static TChild Attach<TParent, TChild>(TParent parent, TChild child)
        where TChild : class, IChild<TParent>
    {
        child.AttachParent(parent);
        return child;
    }

    public static Child<TChild> Create<TParent, TChild>(TParent parent, TChild initialValue)
        where TChild : class, IChild<TParent>
    {
        initialValue.AttachParent(parent);
        return new Child<TChild>(initialValue);
    }

    [return: NotNullIfNotNull(nameof(initialValue))]
    public static Child<TChild>? CreateOptional<TParent, TChild>(TParent parent, TChild? initialValue)
        where TChild : class, IChild<TParent>
    {
        if (initialValue is null)
            return null;
        initialValue.AttachParent(parent);
        return new Child<TChild>(initialValue);
    }
}
