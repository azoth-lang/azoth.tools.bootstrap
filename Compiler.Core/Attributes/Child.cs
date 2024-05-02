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

    [return: NotNullIfNotNull(nameof(initialValue))]
    public static Child<TChild>? Create<TParent, TChild>(TParent parent, TChild? initialValue)
        where TChild : class, IChild<TParent>
    {
        if (initialValue is null)
            return null;
        initialValue.AttachParent(parent);
        return new Child<TChild>(initialValue);
    }
}
