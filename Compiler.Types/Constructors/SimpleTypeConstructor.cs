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
    public IdentifierName? ContainingPackage => null;

    public NamespaceName? ContainingNamespace => null;

    public bool CanBeInstantiated => true;

    public TypeSemantics Semantics => TypeSemantics.Value;
    TypeSemantics? INonVoidAntetype.Semantics => Semantics;

    public SpecialTypeName Name { get; }
    TypeName ITypeConstructor.Name => Name;

    IFixedList<TypeConstructorParameter> ITypeConstructor.Parameters
        => FixedList.Empty<TypeConstructorParameter>();

    public bool AllowsVariance => false;

    IFixedList<GenericParameterPlainType> ITypeConstructor.GenericParameterPlainTypes
        => FixedList.Empty<GenericParameterPlainType>();

    IFixedSet<NamedPlainType> ITypeConstructor.Supertypes => AnyTypeConstructor.Set;

    private protected SimpleTypeConstructor(SpecialTypeName name)
    {
        Name = name;
    }

    public IAntetype Construct(IEnumerable<IAntetype> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Simple type cannot have type arguments", nameof(typeArguments));
        return this;
    }

    public IAntetype TryConstructNullary() => this;

    public IMaybeExpressionAntetype ReplaceTypeParametersIn(IMaybeExpressionAntetype antetype)
        => antetype;

    #region Equality
    public bool Equals(IMaybeExpressionAntetype? other)
        // All simple type constructors are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public bool Equals(ITypeConstructor? other)
        // All simple type constructors are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybeExpressionAntetype other && Equals(other);

    public override int GetHashCode()
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        => base.GetHashCode();
    #endregion

    public sealed override string ToString() => Name.ToString();
}
