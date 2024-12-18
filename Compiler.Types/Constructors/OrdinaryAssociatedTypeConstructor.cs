using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class OrdinaryAssociatedTypeConstructor : AssociatedTypeConstructor
{
    public override bool IsDeclaredConst => false;

    public override TypeSemantics? Semantics => null;

    public override IdentifierName Name { get; }

    public override IFixedSet<BareType> Supertypes => BareType.AnySet;

    public OrdinaryAssociatedTypeConstructor(TypeConstructor containingTypeConstructor, IdentifierName name)
        : base(containingTypeConstructor)
    {
        Name = name;
    }
}
