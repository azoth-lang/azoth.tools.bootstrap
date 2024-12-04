using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The never or bottom type. That is a type with no values. A function
/// with return type `never` must never return by normal means. It always
/// either throws an exception, abandons the process or doesn't terminate.
/// Because it is the bottom type, it is assignment compatible to all types.
/// That makes it particularly useful as the result type of expressions like
/// `throw`, `return` and `break` which never produce a result. It is also
/// used as the type of a `loop` statement with no breaks in it.
/// </summary>
public sealed class NeverPlainType : EmptyPlainType, INonVoidPlainType
{
    #region Singleton
    internal static readonly NeverPlainType Instance = new();

    private NeverPlainType()
        : base(SpecialTypeName.Never) { }
    #endregion

    /// <remarks>Even though <see cref="NeverPlainType"/> is a subtype of all types, this property
    /// returns an empty set because it is not declared with any supertypes.</remarks>
    public override IFixedSet<ConstructedPlainType> Supertypes => [];

    #region Equality
    public override bool Equals(IMaybePlainType? other)
        // NeverPlainType is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(NeverPlainType));
    #endregion
}
