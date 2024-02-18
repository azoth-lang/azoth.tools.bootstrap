using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type that has a capability applied.
/// </summary>
[Closed(typeof(ReferenceType), typeof(ValueType))]
public abstract class CapabilityType : NonEmptyType
{
}
