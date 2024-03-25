using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// A type that has a capability applied.
/// </summary>
[Closed(typeof(ReferenceType), typeof(ValueType))]
public abstract class CapabilityType : NonEmptyType
{
    public Capability Capability { get; }
    public bool IsReadOnlyReference => Capability == Capability.Read;
    public bool IsConstantReference => Capability == Capability.Constant;
    public bool IsTemporarilyConstantReference => Capability == Capability.TemporarilyConstant;
    public bool IsIsolatedReference => Capability == Capability.Isolated;
    public bool IsTemporarilyIsolatedReference => Capability == Capability.TemporarilyIsolated;
    public bool IsIdentityReference => Capability == Capability.Identity;

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

    public IdentifierName? ContainingPackage => DeclaredType.ContainingPackage;

    public NamespaceName ContainingNamespace => DeclaredType.ContainingNamespace;

    public TypeName Name => DeclaredType.Name;

    public IFixedList<DataType> TypeArguments => BareType.GenericTypeArguments;

    public sealed override bool AllowsVariance => BareType.AllowsVariance;

    public sealed override bool HasIndependentTypeArguments => BareType.HasIndependentTypeArguments;

    public IFixedSet<BareReferenceType> Supertypes => BareType.Supertypes;

    public sealed override bool IsFullyKnown => BareType.IsFullyKnown;

    /// <summary>
    /// Whether this type was declared `const` meaning that most references should be treated as
    /// `const`.
    /// </summary>
    public bool IsDeclaredConst => DeclaredType.IsDeclaredConst;

    private protected CapabilityType(Capability capability)
    {
        Capability = capability;
    }

    public override DataType ReplaceTypeParametersIn(DataType type)
        => BareType.ReplaceTypeParametersIn(type);

    public override Pseudotype ReplaceTypeParametersIn(Pseudotype pseudotype)
        => BareType.ReplaceTypeParametersIn(pseudotype);

    public abstract CapabilityType With(Capability capability);

    public sealed override string ToSourceCodeString()
    {
        if (Capability != Capability.Read)
            return $"{Capability.ToSourceCodeString()} {BareType.ToSourceCodeString()}";

        return BareType.ToSourceCodeString();
    }

    public sealed override string ToILString()
        => $"{Capability.ToILString()} {BareType.ToILString()}";
}
