using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type that has a capability applied.
/// </summary>
[Closed(typeof(ReferenceType), typeof(ValueType))]
public abstract class CapabilityType : NonEmptyType
{
    public abstract BareType BareType { get; }

    public virtual DeclaredType DeclaredType => BareType.DeclaredType;

    public IFixedList<DataType> TypeArguments => BareType.TypeArguments;

    public override bool HasIndependentTypeArguments => BareType.HasIndependentTypeArguments;

    public FixedSet<BareReferenceType> Supertypes => BareType.Supertypes;

    public override bool IsFullyKnown => BareType.IsFullyKnown;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// `const`.
    /// </summary>
    public bool IsDeclaredConstant => DeclaredType.IsConstType;

    public override TypeSemantics Semantics => BareType.Semantics;

    private protected CapabilityType() { }
}
