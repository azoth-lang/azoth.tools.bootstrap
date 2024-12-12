using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

/// <summary>
/// An plainType that is defined by its name.
/// </summary>
[Closed(
    typeof(ConstructedOrAssociatedPlainType),
    typeof(GenericParameterPlainType))]
public abstract class ConstructedOrVariablePlainType : NonVoidPlainType
{
    public abstract TypeConstructor? TypeConstructor { get; }
    public abstract TypeName Name { get; }
    public abstract bool AllowsVariance { get; }
    public virtual IFixedList<IPlainType> Arguments => FixedList.Empty<IPlainType>();
    public abstract IFixedSet<ConstructedPlainType> Supertypes { get; }

    internal abstract PlainTypeReplacements TypeReplacements { get; }

    private protected ConstructedOrVariablePlainType() { }

    public abstract string ToBareString();

    public abstract override string ToString();
}
