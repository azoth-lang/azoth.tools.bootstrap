namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

internal sealed class BeforeFirstValueId : IPreviousValueId
{
    private readonly ValueIdScope scope;

    public BeforeFirstValueId(ValueIdScope scope)
    {
        this.scope = scope;
    }

    public ValueId CreateNext() => scope.First;
}
