using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class OrdinaryAssociatedTypeConstructor : AssociatedTypeConstructor
{
    public override bool IsDeclaredConst => false;

    public override TypeSemantics? Semantics { [DebuggerStepThrough] get; }

    public override IdentifierName Name { [DebuggerStepThrough] get; }

    public override BareType? BaseType { [DebuggerStepThrough] get; }

    public override IFixedSet<BareType> Supertypes { [DebuggerStepThrough] get; }

    public override IFixedSet<BareType> Subtypes { [DebuggerStepThrough] get; }

    public OrdinaryAssociatedTypeConstructor(BareTypeConstructor containingTypeConstructor, IdentifierName name)
        : base(containingTypeConstructor)
    {
        Name = name;
        Semantics = null; // Semantics unknown
        BaseType = null;
        Supertypes = BareType.AnySet;
        Subtypes = FixedSet.Empty<BareType>();
    }

    public OrdinaryAssociatedTypeConstructor(BareTypeConstructor containingTypeConstructor, IdentifierName name, BareType equalToType)
        : base(containingTypeConstructor)
    {
        Name = name;
        Semantics = equalToType.Semantics;
        BaseType = equalToType.BaseType;
        Supertypes = equalToType.Supertypes.Prepend(equalToType).ToFixedSet();
        // Subtypes are the same as supertypes because this type is equal to the other type.
        Subtypes = Supertypes;
    }
}
