namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class TypeReplacements
{
    private readonly NonVoidType selfReplacement;

    internal TypeReplacements(NonVoidType type)
    {
        selfReplacement = type;
    }

    public IMaybeType ApplyTo(IMaybeType type)
        => selfReplacement.BareTypeReplacements.ApplyTo(type, selfReplacement);

    public Type ApplyTo(Type type)
        => selfReplacement.BareTypeReplacements.ApplyTo(type, selfReplacement);

    public IMaybeParameterType? ApplyTo(IMaybeParameterType type)
        => selfReplacement.BareTypeReplacements.ApplyTo(type, selfReplacement);

    public ParameterType? ApplyTo(ParameterType type)
        => selfReplacement.BareTypeReplacements.ApplyTo(type, selfReplacement);
}
