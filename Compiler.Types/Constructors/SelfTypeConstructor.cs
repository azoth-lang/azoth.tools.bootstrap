using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

public sealed class SelfTypeConstructor : AssociatedTypeConstructor
{
    public override bool IsDeclaredConst => Context.IsDeclaredConst;

    public override TypeSemantics? Semantics => Context.Semantics;

    public override BuiltInTypeName Name => BuiltInTypeName.Self;

    public override IFixedSet<BareType> Supertypes { [DebuggerStepThrough] get; }

    public SelfTypeConstructor(BareTypeConstructor containingTypeConstructor)
        : base(containingTypeConstructor)
    {
        Supertypes = containingTypeConstructor.Supertypes
            .Prepend(containingTypeConstructor.ConstructWithParameterTypes()).ToFixedSet();
    }
}
