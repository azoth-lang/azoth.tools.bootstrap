using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// An plainType that is defined by its name.
/// </summary>
[Closed(typeof(ConstructedPlainType), typeof(VariablePlainType))]
public abstract class ConstructedOrVariablePlainType : IPlainType
{
    public abstract TypeConstructor? TypeConstructor { get; }
    public abstract TypeName Name { get; }
    public abstract bool AllowsVariance { get; }
    public virtual IFixedList<IPlainType> TypeArguments => FixedList.Empty<IPlainType>();
    public abstract IFixedSet<ConstructedPlainType> Supertypes { get; }

    private protected ConstructedOrVariablePlainType() { }

    public abstract IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType);

    #region Equality
    public abstract bool Equals(IMaybePlainType? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
