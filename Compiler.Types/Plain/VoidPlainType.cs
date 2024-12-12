using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// The void type behaves similar to a unit type. However, it represents the
/// lack of a value. For example, a function returning `void` doesn't return
/// a value. A parameter of type `void` is dropped from the parameter list.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class VoidPlainType : IPlainType
{
    #region Singleton
    internal static readonly VoidPlainType Instance = new VoidPlainType();

    private VoidPlainType() { }
    #endregion

    public TypeConstructor? TypeConstructor => null;
    public TypeSemantics? Semantics => null;
    public SpecialTypeName Name => SpecialTypeName.Void;

    /// <remarks>The void type is an exception to the general rule that all types are a subtype of
    /// <c>Any</c>. Thus, it has no supertypes.</remarks>
    public IFixedSet<ConstructedPlainType> Supertypes => [];

    public IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType) => plainType;

    #region Equality
    public bool Equals(IMaybePlainType? other)
        // This is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(VoidPlainType));
    #endregion

    public override string ToString() => Name.ToString();
}
