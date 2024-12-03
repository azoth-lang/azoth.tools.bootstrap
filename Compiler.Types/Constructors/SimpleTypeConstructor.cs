using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(
    typeof(BoolTypeConstructor),
    typeof(IntegerTypeConstructor))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class SimpleTypeConstructor : SimpleOrLiteralTypeConstructor
{
    public sealed override bool CanBeInstantiated => true;

    public sealed override TypeSemantics Semantics => TypeSemantics.Value;

    public sealed override SpecialTypeName Name { get; }

    public sealed override IFixedList<TypeConstructorParameter> Parameters
        => FixedList.Empty<TypeConstructorParameter>();

    public sealed override bool AllowsVariance => false;

    public sealed override IFixedList<GenericParameterPlainType> GenericParameterPlainTypes
        => FixedList.Empty<GenericParameterPlainType>();

    public sealed override IFixedSet<NamedPlainType> Supertypes => AnyTypeConstructor.Set;

    public OrdinaryNamedPlainType PlainType { get; }

    private protected SimpleTypeConstructor(SpecialTypeName name)
    {
        Name = name;
        PlainType = new(this, []);
    }

    public sealed override IPlainType Construct(IEnumerable<IPlainType> typeArguments)
    {
        if (typeArguments.Any())
            throw new ArgumentException("Simple type cannot have type arguments", nameof(typeArguments));
        return PlainType;
    }

    public sealed override IPlainType TryConstructNullary() => PlainType;

    #region Equality
    public sealed override bool Equals(TypeConstructor? other)
        // All simple type constructors are singletons, so we can use reference equality.
        => ReferenceEquals(this, other);

    public sealed override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public sealed override int GetHashCode()
        => RuntimeHelpers.GetHashCode(this);
    #endregion

    public sealed override string ToString() => Name.ToString();

    public sealed override void ToString(StringBuilder builder) => builder.Append(Name);
}
