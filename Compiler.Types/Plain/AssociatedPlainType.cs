using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OrdinaryAssociatedPlainType),
    typeof(SelfPlainType))]
// TODO separate out the type factory
public abstract class AssociatedPlainType : ConstructedOrAssociatedPlainType, ITypeFactory
{
    public TypeConstructor ContainingType { get; }
    public sealed override TypeConstructor? TypeConstructor => null;
    public sealed override TypeSemantics? Semantics => null;
    public sealed override bool AllowsVariance => false;
    internal sealed override PlainTypeReplacements TypeReplacements => PlainTypeReplacements.None;

    protected AssociatedPlainType(TypeConstructor containingType)
    {
        ContainingType = containingType;
    }

    #region ITypeFactory methods
    PlainType ITypeFactory.TryConstructNullaryPlainType() => this;

    BareType ITypeFactory.TryConstruct(IFixedList<IMaybeType> arguments)
    {
        TypeRequires.NoArgs(arguments, nameof(arguments));
        return new AssociatedBareType(this);
    }

    // Associated types require a capability to construct a full type.
    Type? ITypeFactory.TryConstructNullaryType() => null;
    #endregion

    public sealed override string ToString() => $"{ContainingType}.{Name}";

    public sealed override string ToBareString() => ToString();
}
