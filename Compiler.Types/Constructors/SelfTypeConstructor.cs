using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

/// <summary>
/// Type constructor for the `Self` type.
/// </summary>
public sealed class SelfTypeConstructor : AssociatedTypeConstructor
{
    public override bool IsDeclaredConst => Context.IsDeclaredConst;

    public override TypeSemantics? Semantics => Context.Semantics;

    public override BuiltInTypeName Name => BuiltInTypeName.Self;

    public override BareType? BaseType { [DebuggerStepThrough] get; }

    public override IFixedSet<BareType> Supertypes { [DebuggerStepThrough] get; }

    /// <remarks>
    /// Use <see cref="BareTypeConstructor.SelfTypeConstructor"/> to create/get a <see cref="SelfTypeConstructor"/>.
    /// </remarks>
    internal SelfTypeConstructor(BareTypeConstructor containingTypeConstructor)
        : base(containingTypeConstructor)
    {
        var containingType = containingTypeConstructor.ConstructWithParameterTypes();
        BaseType = containingType.BaseType; // TODO should this also be a self type?
        Supertypes = containingTypeConstructor.Supertypes
            .Prepend(containingType).ToFixedSet();
    }
}
