using System;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class ReferenceCapability
{
    /// <summary>
    /// A reference that has write access and is externally unique. That is,
    /// there are no other references into the object graph reachable from this
    /// reference and no references out from those objects to non-constant
    /// references.
    /// </summary>
    public static readonly ReferenceCapability Isolated = new("iso", allowsWrite: true);

    /// <summary>
    /// An init reference that has write access.
    /// </summary>
    public static readonly ReferenceCapability InitMutable
        = new("init mut", init: true, allowsWrite: true);

    /// <summary>
    /// An init reference that has read-only access.
    /// </summary>
    public static readonly ReferenceCapability InitReadOnly
        = new("init readonly", init: true);

    /// <summary>
    /// A reference that has write access and can be stored into fields etc.
    /// </summary>
    public static readonly ReferenceCapability Mutable
        = new("mut", allowsWrite: true, allowsWriteAliases: true, allowsReadAliases: true);

    /// <summary>
    /// A reference that has read-only access and can be stored into fields etc.
    /// </summary>
    public static readonly ReferenceCapability ReadOnly
        = new("readonly", allowsWriteAliases: true, allowsReadAliases: true);

    /// <summary>
    /// A reference has read-only access and there are no references that
    /// can mutate this object.
    /// </summary>
    public static readonly ReferenceCapability Constant
        = new("const", allowsReadAliases: true);

    /// <summary>
    /// A lent reference that has write access and there are no references that can access this
    /// object while this reference exists.
    /// </summary>
    public static readonly ReferenceCapability LentIsolated = new("lent iso", allowsWrite: true);

    /// <summary>
    /// A lent reference that has write access.
    /// </summary>
    public static readonly ReferenceCapability LentMutable
        = new("lent mut", allowsWrite: true, allowsWriteAliases: true, allowsReadAliases: true);

    /// <summary>
    /// A lent reference that has read-only access.
    /// </summary>
    public static readonly ReferenceCapability LentReadOnly
        = new("lent readonly", allowsWriteAliases: true, allowsReadAliases: true);

    /// <summary>
    /// A lent reference has read-only access and there are no references that
    /// can mutate this object while this reference exists.
    /// </summary>
    public static readonly ReferenceCapability LentConstant
        = new("lent const", allowsReadAliases: true);

    /// <summary>
    /// A reference that can be used to identify an object but not read or
    /// write to it.
    /// </summary>
    public static readonly ReferenceCapability Identity
        = new("id", allowsWriteAliases: true, allowsRead: false, allowsReadAliases: true);

    private readonly string name;

    /// <summary>
    /// Whether this kind of reference is an init reference. Init references always allow assignment
    /// into fields (even `let` fields). They also do not allow aliases. Aliases have the `id`
    /// capability.
    /// </summary>
    public bool IsInit { get; }
    /// <summary>
    /// Whether this kind of reference allows mutating the referenced object through this reference
    /// </summary>
    public bool AllowsWrite { get; }
    /// <summary>
    /// Whether this kind of reference permits other writable aliases to the object to exist
    /// </summary>
    public bool AllowsWriteAliases { get; }
    /// <summary>
    /// Whether this kind of reference allows reading the value of an object through this reference
    /// </summary>
    public bool AllowsRead { get; }
    /// <summary>
    /// Whether this kind of reference permits other readable aliases to the object to exist.
    /// </summary>
    /// <remarks>Note that <see cref="Constant"/> does <see cref="AllowsReadAliases"/> but not
    /// <see cref="AllowsWriteAliases"/>.</remarks>
    public bool AllowsReadAliases { get; }

    /// <summary>
    /// Does this capability allow a reference with it to be recovered to isolated if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsRecoverIsolation => this == Mutable || this == ReadOnly;

    /// <summary>
    /// Does this capability allow a reference with it to be moved if reference sharing permits.
    /// </summary>
    public bool AllowsMove => this == Mutable || this == ReadOnly || this == Isolated;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => this == Mutable || this == ReadOnly || this == Isolated;

    private ReferenceCapability(
        string name,
        bool init = false,
        bool allowsWrite = false,
        bool allowsWriteAliases = false,
        bool allowsRead = true,
        bool allowsReadAliases = false)
    {
        IsInit = init;
        AllowsWrite = allowsWrite;
        AllowsWriteAliases = allowsWriteAliases;
        AllowsRead = allowsRead;
        AllowsReadAliases = allowsReadAliases;
        this.name = name;
    }

    public bool IsAssignableFrom(ReferenceCapability from)
    {
        // Can't gain permissions
        if (AllowsWrite && !from.AllowsWrite) return false;
        if (!AllowsWriteAliases && from.AllowsWriteAliases) return false;
        if (AllowsRead && !from.AllowsRead) return false;
        if (!AllowsReadAliases && from.AllowsReadAliases) return false;

        return true;
    }

    /// <summary>
    /// This capability with any write ability removed.
    /// </summary>
    public ReferenceCapability WithoutWrite()
    {
        // Already not writable. Just return this. That will preserve the correct other attributes
        if (!AllowsWrite) return this;
        // If it is init, there is only one non-writable init capability.
        if (IsInit) return InitReadOnly;
        // It is either `iso` or `mut` either way, convert to `readonly`
        return ReadOnly;
    }

    /// <summary>
    /// The reference capability of a possibly temporary alias to this reference.
    /// </summary>
    public ReferenceCapability Alias() => this == Isolated ? Mutable : this;

    /// <summary>
    /// The reference capability if the referenced object were frozen.
    /// </summary>
    /// <remarks>`id` references remain `id` otherwise they become `const`.</remarks>
    public ReferenceCapability Freeze() => this == Identity ? this : Constant;

    [Obsolete("Use ToSourceCodeString() or ToILString() instead", error: true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        => throw new NotSupportedException();

    public string ToILString() => name;

    public string ToSourceString()
    {
        if (this == ReadOnly) return "⧼read-only⧽";
        if (this == LentReadOnly) return "lent ⧼read-only⧽";
        if (this == InitMutable) return "⧼init⧽ mut";
        if (this == InitReadOnly) return "⧼init⧽ ⧼read-only⧽";
        return name;
    }
}
