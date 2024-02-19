using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
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
    public ReferenceCapability Capability { get; }
    public bool IsReadOnlyReference => Capability == ReferenceCapability.Read;
    public bool IsConstantReference => Capability == ReferenceCapability.Constant;
    public bool IsTemporarilyConstantReference => Capability == ReferenceCapability.TemporarilyConstant;
    public bool IsIsolatedReference => Capability == ReferenceCapability.Isolated;
    public bool IsTemporarilyIsolatedReference => Capability == ReferenceCapability.TemporarilyIsolated;
    public bool IsIdentityReference => Capability == ReferenceCapability.Identity;

    public bool AllowsInit => Capability.AllowsInit;

    public override bool AllowsWrite => Capability.AllowsWrite;

    public override bool AllowsWriteAliases => Capability.AllowsWriteAliases;

    /// <summary>
    /// Does this reference allow it to be recovered to isolated if reference sharing permits.
    /// </summary>
    public bool AllowsRecoverIsolation => Capability.AllowsRecoverIsolation;

    /// <summary>
    /// Does this capability allow a reference with it to be moved if reference sharing permits.
    /// </summary>
    public bool AllowsMove => Capability.AllowsMove && !BareType.IsDeclaredConstType;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => Capability.AllowsFreeze;

    public abstract BareType BareType { get; }

    public virtual DeclaredType DeclaredType => BareType.DeclaredType;

    public SimpleName? ContainingPackage => DeclaredType.ContainingPackage;

    public NamespaceName ContainingNamespace => DeclaredType.ContainingNamespace;

    public TypeName Name => DeclaredType.Name;

    public IFixedList<DataType> TypeArguments => BareType.GenericTypeArguments;

    public sealed override bool HasIndependentTypeArguments => BareType.HasIndependentTypeArguments;

    public FixedSet<BareReferenceType> Supertypes => BareType.Supertypes;

    public sealed override bool IsFullyKnown => BareType.IsFullyKnown;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// `const`.
    /// </summary>
    public bool IsDeclaredConst => DeclaredType.IsDeclaredConst;

    public sealed override TypeSemantics Semantics => BareType.Semantics;

    private protected CapabilityType(ReferenceCapability capability)
    {
        Capability = capability;
    }
}
