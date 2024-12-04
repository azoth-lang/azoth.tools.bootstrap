using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

// TODO possibly remove this class
[Closed(typeof(NeverPlainType), typeof(VoidPlainType))]
public abstract class EmptyPlainType : IPlainType
{
    public TypeConstructor? TypeConstructor => null;
    public TypeSemantics? Semantics => null;
    public SpecialTypeName Name { get; }
    public bool AllowsVariance => false;
    public IFixedList<IPlainType> TypeArguments => [];
    public abstract IFixedSet<ConstructedPlainType> Supertypes { get; }

    private protected EmptyPlainType(SpecialTypeName name)
    {
        Name = name;
    }

    public IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType) => plainType;

    #region Equality
    public abstract bool Equals(IMaybePlainType? other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public sealed override string ToString() => Name.ToString();
}
