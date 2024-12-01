using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(BoolTypeConstructor),
    typeof(NumericTypeConstructor))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class SimpleTypeConstructor : INonVoidAntetype, ITypeConstructor, ISimpleOrConstValueAntetype
{
    public bool CanBeConstructed => true;

    public SpecialTypeName Name { get; }

    public bool HasReferenceSemantics => false;

    IFixedList<AntetypeGenericParameter> ITypeConstructor.GenericParameters
        => FixedList.Empty<AntetypeGenericParameter>();

    public bool AllowsVariance => false;

    IFixedList<GenericParameterAntetype> ITypeConstructor.GenericParameterAntetypes
        => FixedList.Empty<GenericParameterAntetype>();

    private protected SimpleTypeConstructor(SpecialTypeName name)
    {
        Name = name;
    }

    public IAntetype With(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Simple type cannot have type arguments", nameof(typeArguments));
        return this;
    }

    public IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other)
        // All simple antetypes are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public bool Equals(ITypeConstructor? other)
        // All simple antetypes are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode()
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        => base.GetHashCode();
    #endregion

    public sealed override string ToString() => Name.ToString();
}
