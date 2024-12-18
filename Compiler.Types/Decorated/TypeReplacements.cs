namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class TypeReplacements
{
    private readonly NonVoidType selfReplacement;

    internal TypeReplacements(NonVoidType type)
    {
        selfReplacement = type;
    }

    public IMaybeType Apply(IMaybeType type)
        => selfReplacement.BareTypeReplacements.Apply(type, selfReplacement);

    public Type Apply(Type type)
        => selfReplacement.BareTypeReplacements.Apply(type, selfReplacement);

    public IMaybeParameterType? Apply(IMaybeParameterType type)
        => selfReplacement.BareTypeReplacements.Apply(type, selfReplacement);

    public ParameterType? Apply(ParameterType type)
        => selfReplacement.BareTypeReplacements.Apply(type, selfReplacement);
}
