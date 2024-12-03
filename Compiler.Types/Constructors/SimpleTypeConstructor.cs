using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(BoolTypeConstructor),
    typeof(NumericTypeConstructor))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class SimpleTypeConstructor : ISimpleOrLiteralTypeConstructor
{
    public IdentifierName? ContainingPackage => null;

    public NamespaceName? ContainingNamespace => null;

    public bool CanBeInstantiated => true;

    public TypeSemantics Semantics => TypeSemantics.Value;

    public SpecialTypeName Name { get; }
    TypeName ITypeConstructor.Name => Name;

    IFixedList<TypeConstructorParameter> ITypeConstructor.Parameters
        => FixedList.Empty<TypeConstructorParameter>();

    bool ITypeConstructor.AllowsVariance => false;

    IFixedList<GenericParameterPlainType> ITypeConstructor.GenericParameterPlainTypes
        => FixedList.Empty<GenericParameterPlainType>();

    IFixedSet<NamedPlainType> ITypeConstructor.Supertypes => AnyTypeConstructor.Set;

    public OrdinaryNamedPlainType PlainType { get; }

    private protected SimpleTypeConstructor(SpecialTypeName name)
    {
        Name = name;
        PlainType = new(this, []);
    }

    public IPlainType Construct(IEnumerable<IPlainType> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Simple type cannot have type arguments", nameof(typeArguments));
        return PlainType;
    }

    public IPlainType TryConstructNullary() => PlainType;

    #region Equality
    public bool Equals(ITypeConstructor? other)
        // All simple type constructors are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public override int GetHashCode()
        => RuntimeHelpers.GetHashCode(this);
    #endregion

    public sealed override string ToString() => Name.ToString();
}
