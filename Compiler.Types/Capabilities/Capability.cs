using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class Capability : ICapabilityConstraint
{
    /// <summary>
    /// A reference that has write access and is the sole reference into an isolated sub-graph. That
    /// is, there are no other references into the object graph reachable from this reference and no
    /// references out from those objects to non-constant objects.
    /// </summary>
    public static readonly Capability Isolated = new("iso", allowsWrite: true);

    /// <summary>
    /// A reference that has write access and is <i>temporarily</i> the sole reference into an
    /// isolated sub-graph. That is, there are no other references into the object graph reachable
    /// from this reference and no references out from those objects to non-constant objects. Except
    /// that there can be sequestered aliases into the object graph.
    /// </summary>
    public static readonly Capability TemporarilyIsolated
        = new("temp iso", allowsWrite: true, allowsSequesteredAliases: true);

    /// <summary>
    /// A reference that has write access and can be stored into fields etc.
    /// </summary>
    public static readonly Capability Mutable
        = new("mut", allowsWrite: true, allowsWriteAliases: true, allowsReadAliases: true);

    /// <summary>
    /// An init reference that has write access.
    /// </summary>
    public static readonly Capability InitMutable
        = new("init mut", "⧼init⧽ mut", init: true, allowsWrite: true);

    /// <summary>
    /// A reference that has read-only access and can be stored into fields etc.
    /// </summary>
    public static readonly Capability Read
        = new("read", "read", allowsWriteAliases: true, allowsReadAliases: true);

    /// <summary>
    /// An init reference that has read-only access.
    /// </summary>
    public static readonly Capability InitReadOnly
        = new("init read", "⧼init⧽ read", init: true);

    /// <summary>
    /// A reference that has read-only access and there are no references that
    /// can mutate this object.
    /// </summary>
    public static readonly Capability Constant
        = new("const", allowsReadAliases: true);

    /// <summary>
    /// A reference that has read-only access and there are <i>temporarily</i> no references that
    /// can mutate this object. There can be sequestered references that can mutate the object.
    /// </summary>
    public static readonly Capability TemporarilyConstant
        = new("temp const", allowsReadAliases: true, allowsSequesteredAliases: true);

    /// <summary>
    /// A reference that can be used to identify an object but not read or
    /// write to it.
    /// </summary>
    public static readonly Capability Identity
        = new("id", allowsWriteAliases: true, allowsRead: false, allowsReadAliases: true);

    public static IComparer<Capability> SubtypeComparer = Comparer<Capability>.Create((a, b) =>
    {
        if (ReferenceEquals(a, b)) return 0;
        if (a.IsSubtypeOf(b)) // i.e. a < b
            return -1;
        return 1;
    });

    private readonly string ilName;
    private readonly string sourceCodeName;

    /// <summary>
    /// Whether this kind of reference is an init reference. Init references always permit assignment
    /// into fields (even `let` fields). They also do not permit aliases. Aliases have the `id`
    /// capability.
    /// </summary>
    public bool AllowsInit { [DebuggerStepThrough] get; }
    /// <summary>
    /// Whether this kind of reference permits mutating the referenced object through this reference.
    /// </summary>
    public bool AllowsWrite { [DebuggerStepThrough] get; }
    bool ICapabilityConstraint.AnyCapabilityAllowsWrite => AllowsWrite;
    /// <summary>
    /// Whether this kind of reference permits other writable aliases to the object to exist.
    /// </summary>
    public bool AllowsWriteAliases { [DebuggerStepThrough] get; }
    /// <summary>
    /// Whether this kind of reference permits reading the value of an object through this reference.
    /// </summary>
    public bool AllowsRead { [DebuggerStepThrough] get; }
    /// <summary>
    /// Whether this kind of reference permits other readable aliases to the object to exist.
    /// </summary>
    /// <remarks>Note that <see cref="Constant"/> does <see cref="AllowsReadAliases"/> but not
    /// <see cref="AllowsWriteAliases"/>.</remarks>
    public bool AllowsReadAliases { [DebuggerStepThrough] get; }
    /// <summary>
    /// Whether this kind of reference permits aliases that have been sequestered to temporarily
    /// strengthen the capability of the reference. This is only true for `temp iso` and `temp const`.
    /// </summary>
    public bool AllowsSequesteredAliases { [DebuggerStepThrough] get; }

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

    private Capability(
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

    private Capability(
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
    public bool IsAssignableFrom(Capability from)
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

    public bool IsSubtypeOf(ICapabilityConstraint other) => other.IsAssignableFrom(this);

    /// <summary>
    /// Can a reference with this capability be assigned from a reference with the given capability
    /// constraint ignoring lent.
    /// </summary>
    /// <remarks>This ignores `lent` because "swapping" means there are times where a lent reference
    /// is passed to something expecting a non-lent reference. The rules are context dependent.</remarks>
    public bool IsAssignableFrom(ICapabilityConstraint from)
        => from switch
        {
            Capability c => IsAssignableFrom(c),
            CapabilitySet c => IsAssignableFrom(c.UpperBound),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    /// <summary>
    /// This capability with any write ability removed.
    /// </summary>
    public Capability WithoutWrite()
    {
        // Already not writable. Just return this. That will preserve the correct other attributes
        if (!AllowsWrite) return this;
        // If it is init, there is only one non-writable init capability.
        if (AllowsInit) return InitReadOnly;
        // It is either `iso`, `temp iso`, or `mut`. Regardless, convert to `readonly`
        return Read;
    }

    /// <summary>
    /// The effective capability on a member with the current capability when it is accessed from a
    /// value with the given capability.
    /// </summary>
    public ICapabilityConstraint AccessedVia(ICapabilityConstraint capability)
        => capability switch
        {
            Capability c => AccessedVia(c),
            CapabilitySet c => AccessedVia(c),
            _ => throw ExhaustiveMatch.Failed(),
        };

    /// <summary>
    /// The effective capability on a member with the current capability when it is accessed from a
    /// value with the given capability.
    /// </summary>
    public Capability AccessedVia(Capability capability)
    {
        if (AllowsInit)
            throw new InvalidOperationException("Fields cannot have the init capability.");

        if (capability == Identity)
            return this == Constant ? Constant : Identity;
        if (this == Identity) return Identity;

        // Constant is contagious
        if (capability == Constant || this == Constant) return Constant;
        if (capability == TemporarilyConstant) return TemporarilyConstant;

        if (capability == Isolated || capability == TemporarilyIsolated)
            throw new InvalidOperationException("Cannot access via isolated, must alias first.");

        if (!capability.AllowsWrite)
            return WithoutWrite();

        return this;
    }

    /// <summary>
    /// The effective capability on a member with the current capability when it is accessed from a
    /// value with the given capability set.
    /// </summary>
    public ICapabilityConstraint AccessedVia(CapabilitySet capabilitySet)
        // TODO is there a more efficient way to do this?
        => (ICapabilityConstraint?)capabilitySet.AllowedCapabilities
                                                .Select(c => AccessedVia(c.OfAlias()))
                                                .Distinct().TrySingle() ?? capabilitySet;

    /// <summary>
    /// The reference capability after a possibly temporary alias has been made to it.
    /// </summary>
    /// <remarks>Even though the behavior is the same as <see cref="OfAlias"/> the operation is
    /// logically distinct.</remarks>
    public Capability WhenAliased()
        => this == Isolated || this == TemporarilyIsolated ? Mutable : this;

    /// <summary>
    /// The reference capability of an alias to this type.
    /// </summary>
    /// <remarks>Even though the behavior is the same as <see cref="WhenAliased"/> the operation is
    /// logically distinct.</remarks>
    public Capability OfAlias()
        => this == Isolated || this == TemporarilyIsolated ? Mutable : this;

    /// <summary>
    /// The reference capability if the referenced object were frozen.
    /// </summary>
    /// <remarks>`id` references remain `id` otherwise they become `const`.</remarks>
    public Capability Freeze() => this == Identity ? this : Constant;

    /// <summary>
    /// Upcast this capability to the lowest compatible capability in the given capability set.
    /// </summary>
    /// <remarks>This operates without allowing a freeze so `iso` cannot become `const`.</remarks>
    public Capability? UpcastTo(CapabilitySet capabilitySet)
    {
        if (capabilitySet.AllowedCapabilities.Contains(this))
            // It is already in the set
            return this;
        return capabilitySet.AllowedCapabilities.Where(IsSubtypeOf).Min(SubtypeComparer);
    }

    /// <summary>
    /// Convert to a capability that is appropriate for the given bare type based on whether the
    /// type is declared `const`.
    /// </summary>
    public Capability ToCapabilityFor(BareTypeConstructor typeConstructor)
    {
        if (typeConstructor.IsDeclaredConst && this == Read) return Constant;
        return this;
    }

    public override string ToString()
        => ToILString();

    public string ToILString() => ilName;

    public string ToSourceCodeString() => sourceCodeName;
}
