using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A reference type without any capability attached
/// </summary>
[Closed(
    typeof(BareObjectType),
    typeof(BareAnyType))]
public abstract class BareReferenceType
{
    public abstract ReferenceType With(ReferenceCapability capability);
}
