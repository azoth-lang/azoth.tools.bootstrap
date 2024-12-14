using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OrdinaryAssociatedPlainType),
    typeof(SelfPlainType))]
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

    BareType ITypeFactory.TryConstruct(IFixedList<IMaybeType> arguments)
    {
        if (arguments.IsEmpty()) throw new ArgumentException("Associated type cannot have type arguments", nameof(arguments));
        return new AssociatedBareType(this);
    }

    public sealed override string ToString() => $"{ContainingType}.{Name}";

    public sealed override string ToBareString() => ToString();
}
