using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(NeverPlainType), typeof(VoidPlainType))]
public abstract class EmptyPlainType : NamedPlainType
{
    public sealed override ITypeConstructor? TypeConstructor => null;
    public TypeSemantics? Semantics => null;
    /// <summary>
    /// Empty types aren't abstract, but they still can't be instantiated.
    /// </summary>
    public bool CanBeInstantiated => false;
    public override SpecialTypeName Name { get; }
    public sealed override bool AllowsVariance => false;

    private protected EmptyPlainType(SpecialTypeName name)
    {
        Name = name;
    }

    public override IMaybeAntetype ReplaceTypeParametersIn(IMaybeAntetype antetype) => antetype;

    public sealed override string ToString() => Name.ToString();
}
