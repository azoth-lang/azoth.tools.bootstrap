using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(typeof(CapabilityViewpointType))]
public abstract class ViewpointType : NonEmptyType
{
    protected ViewpointType(GenericParameterType referent)
    {
        Referent = referent;
    }

    public GenericParameterType Referent { get; }

    public override bool IsFullyKnown => Referent.IsFullyKnown;

    public override TypeSemantics Semantics => Referent.Semantics;
}
