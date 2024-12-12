using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(typeof(ConstructedPlainType), typeof(AssociatedPlainType))]
public abstract class ConstructedOrAssociatedPlainType : NonVoidPlainType
{
    public abstract TypeConstructor? TypeConstructor { get; }
    public abstract TypeName Name { get; }
    public abstract bool AllowsVariance { get; }
    public virtual IFixedList<IPlainType> Arguments => FixedList.Empty<IPlainType>();
    public abstract IFixedSet<ConstructedPlainType> Supertypes { get; }

    internal abstract PlainTypeReplacements TypeReplacements { get; }

    private protected ConstructedOrAssociatedPlainType() { }

    public abstract string ToBareString();

    public abstract override string ToString();
}
