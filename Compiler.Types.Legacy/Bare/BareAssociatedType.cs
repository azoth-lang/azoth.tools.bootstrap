using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;

/// <summary>
/// The type of a type variable that is inherently bare (i.e. a <see cref="SelfType"/> or an
/// <c>OrdinaryAssociatedType</c>).
/// </summary>
[Closed(typeof(SelfType))]
public abstract class BareAssociatedType : BareType
{
    public sealed override TypeConstructor? TypeConstructor => null;
    /// <remarks>Associated types are effectively not declared const because they could be inhabited
    /// by non-const types.</remarks>
    public override bool IsDeclaredConst => false;
    public override bool AllowsVariance => false;
    public override bool HasIndependentTypeArguments => false;
    public sealed override IFixedList<IType> TypeArguments => [];
    public sealed override IEnumerable<GenericParameterArgument> GenericParameterArguments => [];
    public abstract override VariablePlainType ToPlainType();

    public override IType ReplaceTypeParametersIn(IType type) => type;
    public override IMaybeType ReplaceTypeParametersIn(IMaybeType type) => type;
    public override IPseudotype ReplaceTypeParametersIn(IPseudotype pseudotype) => pseudotype;
    public override IMaybePseudotype ReplaceTypeParametersIn(IMaybePseudotype pseudotype) => pseudotype;

    public override BareAssociatedType AccessedVia(Capability capability) => this;

    public override BareAssociatedType With(IFixedList<IType> typeArguments)
    {
        Requires.That(typeArguments.IsEmpty, nameof(typeArguments), "Cannot apply type arguments to associated type.");
        return this;
    }
}
