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
