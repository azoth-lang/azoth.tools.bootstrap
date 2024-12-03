using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(NeverPlainType), typeof(VoidPlainType))]
public abstract class EmptyPlainType : NamedPlainType
{
    public sealed override TypeConstructor? TypeConstructor => null;
    public TypeSemantics? Semantics => null;
    public override SpecialTypeName Name { get; }
    public sealed override bool AllowsVariance => false;

    private protected EmptyPlainType(SpecialTypeName name)
    {
        Name = name;
    }

    public override IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType) => plainType;

    public sealed override string ToString() => Name.ToString();
}
