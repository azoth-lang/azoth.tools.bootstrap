using System;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

[DebuggerDisplay("{ToILString()}")]
public sealed class ReferenceCapability : IReferenceCapabilityConstraint
{
    /// <summary>
    /// A reference that has write access and is the sole reference into an isolated sub-graph. That
    /// is, there are no other references into the object graph reachable from this reference and no
    /// references out from those objects to non-constant objects.
    /// </summary>
    public static readonly ReferenceCapability Isolated = new("iso", allowsWrite: true);

    /// <summary>
    /// A reference that has write access and is <i>temporarily</i> the sole reference into an
    /// isolated sub-graph. That is, there are no other references into the object graph reachable
    /// from this reference and no references out from those objects to non-constant objects. Except
    /// that there can be sequestered aliases into the object graph.
    /// </summary>
    public static readonly ReferenceCapability TemporarilyIsolated
        = new("temp iso", allowsWrite: true, allowsSequesteredAliases: true);

    /// <summary>
    /// A reference that has write access and can be stored into fields etc.
    /// </summary>
    public static readonly ReferenceCapability Mutable
        = new("mut", allowsWrite: true, allowsWriteAliases: true, allowsReadAliases: true);

    /// <summary>
    /// An init reference that has write access.
    /// </summary>
    public static readonly ReferenceCapability InitMutable
        = new("init mut", "⧼init⧽ mut", init: true, allowsWrite: true);

    /// <summary>
    /// A reference that has read-only access and can be stored into fields etc.
    /// </summary>
    public static readonly ReferenceCapability Read
        = new("read", "read", allowsWriteAliases: true, allowsReadAliases: true);

    /// <summary>
    /// An init reference that has read-only access.
    /// </summary>
    public static readonly ReferenceCapability InitReadOnly
        = new("init read", "⧼init⧽ read", init: true);

    /// <summary>
    /// A reference that has read-only access and there are no references that
    /// can mutate this object.
    /// </summary>
    public static readonly ReferenceCapability Constant
        = new("const", allowsReadAliases: true);

    /// <summary>
    /// A reference that has read-only access and there are <i>temporarily</i> no references that
    /// can mutate this object. There can be sequestered references that can mutate the object.
    /// </summary>
    public static readonly ReferenceCapability TemporarilyConstant
        = new("temp const", allowsReadAliases: true, allowsSequesteredAliases: true);

    /// <summary>
    /// A reference that can be used to identify an object but not read or
    /// write to it.
    /// </summary>
    public static readonly ReferenceCapability Identity
        = new("id", allowsWriteAliases: true, allowsRead: false, allowsReadAliases: true);

    private readonly string ilName;
    private readonly string sourceCodeName;

    /// <summary>
    /// Whether this kind of reference is an init reference. Init references always permit assignment
    /// into fields (even `let` fields). They also do not permit aliases. Aliases have the `id`
    /// capability.
    /// </summary>
    public bool AllowsInit { get; }
    /// <summary>
    /// Whether this kind of reference permits mutating the referenced object through this reference.
    /// </summary>
    public bool AllowsWrite { get; }
    /// <summary>
    /// Whether this kind of reference permits other writable aliases to the object to exist.
    /// </summary>
    public bool AllowsWriteAliases { get; }
    /// <summary>
    /// Whether this kind of reference permits reading the value of an object through this reference.
    /// </summary>
    public bool AllowsRead { get; }
    /// <summary>
    /// Whether this kind of reference permits other readable aliases to the object to exist.
    /// </summary>
    /// <remarks>Note that <see cref="Constant"/> does <see cref="AllowsReadAliases"/> but not
    /// <see cref="AllowsWriteAliases"/>.</remarks>
    public bool AllowsReadAliases { get; }
    /// <summary>
    /// Whether this kind of reference permits aliases that have been sequestered to temporarily
    /// strengthen the capability of the reference. This is only true for `temp iso` and `temp const`.
    /// </summary>
    public bool AllowsSequesteredAliases { get; }

    /// <summary>
    /// Does this capability allow a reference with it to be recovered to isolated if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsRecoverIsolation => this == Mutable || this == Read;

    /// <summary>
    /// Does this capability allow a reference with it to be moved into isolated if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsMove => this == Mutable || this == Read || this == Isolated || this == TemporarilyIsolated;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => this == Mutable || this == Read || this == Isolated;

    private ReferenceCapability(
        string name,
        bool init = false,
        bool allowsWrite = false,
        bool allowsWriteAliases = false,
        bool allowsRead = true,
        bool allowsReadAliases = false,
        bool allowsSequesteredAliases = false)
        : this(name, name, init, allowsWrite, allowsWriteAliases, allowsRead, allowsReadAliases,
            allowsSequesteredAliases)
    {
    }

    private ReferenceCapability(
        string ilName,
        string sourceCodeName,
        bool init = false,
        bool allowsWrite = false,
        bool allowsWriteAliases = false,
        bool allowsRead = true,
        bool allowsReadAliases = false,
        bool allowsSequesteredAliases = false)
    {
        this.ilName = ilName;
        this.sourceCodeName = sourceCodeName;
        AllowsInit = init;
        AllowsWrite = allowsWrite;
        AllowsWriteAliases = allowsWriteAliases;
        AllowsRead = allowsRead;
        AllowsReadAliases = allowsReadAliases;
        AllowsSequesteredAliases = allowsSequesteredAliases;
    }

    /// <summary>
    /// Can a reference with this capability be assigned from a reference with the given capability
    /// ignoring lent.
    /// </summary>
    /// <remarks>This ignores `lent` because "swapping" means there are times where a lent reference
    /// is passed to something expecting a non-lent reference. The rules are context dependent.</remarks>
    public bool IsAssignableFrom(ReferenceCapability from)
    {
        // Can't change `init`
        if (AllowsInit != from.AllowsInit) return false;
        // Can't gain permissions
        if (AllowsWrite && !from.AllowsWrite) return false;
        if (!AllowsWriteAliases && from.AllowsWriteAliases) return false;
        if (AllowsRead && !from.AllowsRead) return false;
        if (!AllowsReadAliases && from.AllowsReadAliases) return false;
        // Can't change `temp` to non-temp when target disallows aliases
        if (!AllowsSequesteredAliases && from.AllowsSequesteredAliases
            && (!AllowsWriteAliases || !AllowsReadAliases)) return false;

        return true;
    }

    public bool IsAssignableFrom(IReferenceCapabilityConstraint from)
    {
        return from switch
        {
            ReferenceCapability fromCapability => IsAssignableFrom(fromCapability),
            ReferenceCapabilityConstraint _ => false,
            _ => throw ExhaustiveMatch.Failed(from),
        };
    }

    /// <summary>
    /// This capability with any write ability removed.
    /// </summary>
    public ReferenceCapability WithoutWrite()
    {
        // Already not writable. Just return this. That will preserve the correct other attributes
        if (!AllowsWrite) return this;
        // If it is init, there is only one non-writable init capability.
        if (AllowsInit) return InitReadOnly;
        // It is either `iso`, `temp iso`, or `mut`. Regardless, convert to `readonly`
        return Read;
    }

    public ReferenceCapability AccessedVia(ReferenceCapability capability)
    {
        if (AllowsInit)
            throw new InvalidOperationException("Fields cannot have the init capability.");
        if (capability == Identity)
            return this == Constant ? Constant : Identity;

        // Constant is contagious
        if (capability == Constant) return Constant;
        if (capability == TemporarilyConstant) return TemporarilyConstant;

        if (capability == Isolated || capability == TemporarilyIsolated)
            throw new NotImplementedException("prevent breaking isolation");

        if (!capability.AllowsWrite)
            return WithoutWrite();

        return this;
    }

    /// <summary>
    /// The reference capability after a possibly temporary alias has been made to it.
    /// </summary>
    /// <remarks>Even though the behavior is the same as <see cref="OfAlias"/> the operation is
    /// logically distinct.</remarks>
    public ReferenceCapability WhenAliased()
        => this == Isolated ? Mutable : this;

    /// <summary>
    /// The reference capability of an alias to this type.
    /// </summary>
    /// <remarks>Even though the behavior is the same as <see cref="WhenAliased"/> the operation is
    /// logically distinct.</remarks>
    public ReferenceCapability OfAlias()
    {
        if (this == Isolated) return Mutable;
        return this;
    }

    /// <summary>
    /// The reference capability if the referenced object were frozen.
    /// </summary>
    /// <remarks>`id` references remain `id` otherwise they become `const`.</remarks>
    public ReferenceCapability Freeze() => this == Identity ? this : Constant;

    public override string ToString()
        => throw new NotSupportedException();

    public string ToILString() => ilName;

    public string ToSourceString() => sourceCodeName;
}
