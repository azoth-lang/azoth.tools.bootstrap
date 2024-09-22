using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(CapabilityViewpointType), typeof(SelfViewpointType))]
public abstract class ViewpointType : NonEmptyType, INonVoidType
{
    public abstract ICapabilityConstraint Capability { get; }

    public abstract INonVoidType Referent { get; }

    private protected ViewpointType() { }

    IMaybeNonVoidType IMaybeNonVoidType.WithoutWrite() => this;

    public sealed override INonVoidAntetype ToAntetype() => Referent.ToAntetype();

    public sealed override IType AccessedVia(ICapabilityConstraint capability) => this;
}
