using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes;

/// <summary>
/// The never or bottom type. That is a type with no values. A function
/// with return type `never` must never return by normal means. It always
/// either throws an exception, abandons the process or doesn't terminate.
/// Because it is the bottom type, it is assignment compatible to all types
/// this makes it particularly useful as the result type of expressions like
/// `throw`, `return` and `break` which never produce a result. It is also
/// used as the type of a `loop` statement with no breaks in it.
/// </summary>
public sealed class NeverAntetype : EmptyAntetype, INonVoidAntetype
{
    #region Singleton
    internal static readonly NeverAntetype Instance = new();

    private NeverAntetype()
        : base(SpecialTypeName.Never) { }
    #endregion

    /// <remarks>Even though <see cref="NeverAntetype"/> is a subtype of all types, this property
    /// returns an empty set because it is not declared with any supertypes.</remarks>
    public override IFixedSet<NominalAntetype> Supertypes => FixedSet.Empty<NominalAntetype>();

    // TODO this is especially strange because it is a subtype of reference types
    public bool HasReferenceSemantics => false;

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
        // NeverAntetype is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(NeverAntetype));
    #endregion
}
