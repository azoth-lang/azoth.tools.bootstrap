using System;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class ReferenceCapability
{
    /// <summary>
    /// A reference that has write access and is externally unique. That is,
    /// there are no other references into the object reachable from this
    /// reference and no references out from those objects to non-constant
    /// references.
    /// </summary>
    public static readonly ReferenceCapability Isolated = new("iso", allowsWrite: true);

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
    /// A reference that can be used to identify an object but not read or
    /// write to it.
    /// </summary>
    public static readonly ReferenceCapability Identity
        = new("id", allowsWriteAliases: true, allowsRead: false, allowsReadAliases: true);

    private readonly string name;
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
    /// Whether this kind of reference permits other readable aliases to the object to exist
    /// </summary>
    // TODO merge with AllowsWriteAliases to just AllowsAliases?
    public bool AllowsReadAliases { get; }

    public bool AllowsMovable => AllowsWrite && !AllowsWriteAliases;

    /// <summary>
    /// Does this capability allow a reference with it to be recovered to isolated if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsRecoverIsolation => AllowsRead && AllowsReadAliases;

    /// <summary>
    /// Does this capability allow a reference with it to be frozen to const if reference
    /// sharing permits.
    /// </summary>
    public bool AllowsFreeze => AllowsRead;

    private ReferenceCapability(
        string name,
        bool allowsWrite = false,
        bool allowsWriteAliases = false,
        bool allowsRead = true,
        bool allowsReadAliases = false)
    {
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
    /// <returns></returns>
    public ReferenceCapability WithoutWrite()
    {
        // Already not writable. Just return this. That will preserve the correct other attributes
        if (!AllowsWrite) return this;
        // It is either `iso` or `mut` either way, convert to `readonly`
        return ReadOnly;
    }

    [Obsolete("Use ToSourceCodeString() or ToILString() instead", error: true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public sealed override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        =>
            throw new NotSupportedException();

    public string ToILString() => name;

    public string ToSourceString() => this == ReadOnly ? "⧼read-only⧽" : name;
}
