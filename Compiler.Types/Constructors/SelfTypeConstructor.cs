using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class SelfTypeConstructor : AssociatedTypeConstructor
{
    public override bool IsDeclaredConst => Context.IsDeclaredConst;

    public override TypeSemantics? Semantics { [DebuggerStepThrough] get; }

    public override BuiltInTypeName Name => BuiltInTypeName.Self;

    public override IFixedSet<BareType> Supertypes { [DebuggerStepThrough] get; }

    public SelfTypeConstructor(BareTypeConstructor containingTypeConstructor)
        : base(containingTypeConstructor)
    {
        Semantics = ComputeSemantics(containingTypeConstructor);
        Supertypes = containingTypeConstructor.Supertypes
            .Prepend(containingTypeConstructor.ConstructWithParameterTypes()).ToFixedSet();
    }

    private TypeSemantics? ComputeSemantics(BareTypeConstructor typeConstructor)
    {
        // TODO this all seems correct, but is it really necessary?
        if (Context is not OrdinaryTypeConstructor ordinaryTypeConstructor) return null;

        return ordinaryTypeConstructor.Kind switch
        {
            TypeKind.Class => TypeSemantics.Reference,
            TypeKind.Struct => TypeSemantics.Value,
            TypeKind.Trait => null,
            _ => throw ExhaustiveMatch.Failed(ordinaryTypeConstructor.Kind),
        };
    }
}
