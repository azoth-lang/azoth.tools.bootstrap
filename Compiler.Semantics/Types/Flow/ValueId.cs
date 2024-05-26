namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

public record struct ValueId(ValueIdScope Scope, ulong Value)
{
    public readonly override string ToString() => $"⧼value{Value}⧽";
}
