using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

[Closed(typeof(CapabilityViewpointType), typeof(SelfViewpointType))]
public abstract class ViewpointType : NonEmptyType, INonVoidType
{
    public abstract ICapabilityConstraint Capability { get; }

    public abstract INonVoidType Referent { get; }

    private protected ViewpointType() { }

    IMaybeNonVoidType IMaybeNonVoidType.WithoutWrite() => this;

    IMaybeType IMaybeType.AccessedVia(IMaybePseudotype contextType) => (IMaybeType)AccessedVia(contextType);

    public sealed override IType AccessedVia(ICapabilityConstraint capability) => this;
    IMaybeType IMaybeType.AccessedVia(ICapabilityConstraint capability) => AccessedVia(capability);
}
