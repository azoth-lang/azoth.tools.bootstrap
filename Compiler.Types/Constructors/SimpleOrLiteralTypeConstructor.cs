using System.Diagnostics;
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
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class SimpleOrLiteralTypeConstructor : TypeConstructor
{
    TypeConstructorContext TypeConstructor.Context => PrimitiveContext.Instance;

    public bool IsDeclaredConst => true;

    public abstract bool CanHaveFields { get; }

    public abstract bool CanBeInstantiated { get; }

    public bool CanBeSupertype => false;

    public abstract TypeSemantics Semantics { get; }

    public abstract TypeName Name { get; }

    public bool HasParameters => !Parameters.IsEmpty;
    public abstract IFixedList<TypeConstructor.Parameter> Parameters { get; }

    public abstract bool AllowsVariance { get; }
    public abstract bool HasIndependentParameters { get; }

    public abstract IFixedList<GenericParameterPlainType> ParameterPlainTypes { get; }

    public abstract IFixedSet<TypeConstructor.Supertype> Supertypes { get; }

    public abstract ConstructedPlainType Construct(IFixedList<IPlainType> typeArguments);
    public abstract bool Equals(TypeConstructor? other);
    public abstract IPlainType? TryConstructNullary();

    public abstract override string ToString();

    public abstract void ToString(StringBuilder builder);
}
