using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

// TODO this should probably be removed. It seems to be used only for improperly handling operators yielding these types
[Closed(
    typeof(SimpleTypeConstructor),
    typeof(LiteralTypeConstructor))]
public abstract class SimpleOrLiteralTypeConstructor : TypeConstructor
{
    TypeConstructorContext TypeConstructor.Context => PrimitiveContext.Instance;

    public abstract bool CanBeInstantiated { get; }

    public abstract TypeSemantics Semantics { get; }

    public abstract TypeName Name { get; }

    public abstract IFixedList<TypeConstructorParameter> Parameters { get; }

    public abstract bool AllowsVariance { get; }

    public abstract IFixedList<GenericParameterPlainType> GenericParameterPlainTypes { get; }

    public abstract IFixedSet<OrdinaryNamedPlainType> Supertypes { get; }

    public abstract IPlainType Construct(IEnumerable<IPlainType> typeArguments);
    public abstract bool Equals(TypeConstructor? other);
    public abstract IPlainType? TryConstructNullary();

    public abstract override string ToString();

    public abstract void ToString(StringBuilder builder);
}
