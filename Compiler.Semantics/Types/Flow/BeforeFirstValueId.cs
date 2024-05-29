using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

internal sealed class BeforeFirstValueId : IPreviousValueId
{
    private readonly ValueIdScope scope;

    public BeforeFirstValueId(ValueIdScope scope)
    {
        this.scope = scope;
    }

    public ValueId CreateNext()
    {
        var first = scope.CreateValueId();
        if (first.Value != 0)
            throw new InvalidOperationException("First value Id must be 0.");
        return first;
    }
}
