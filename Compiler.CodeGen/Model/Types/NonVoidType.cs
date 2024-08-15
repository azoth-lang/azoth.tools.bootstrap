using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

// TODO merge up into Type
[Closed(typeof(NonOptionalType), typeof(OptionalType))]
public abstract class NonVoidType : Type
{
    public Symbol UnderlyingSymbol { get; }

    private protected NonVoidType(Symbol underlyingSymbol)
    {
        UnderlyingSymbol = underlyingSymbol;
    }

    public abstract override NonVoidType WithSymbol(Symbol symbol);

    /// <summary>
    /// Convert to an outer type that is not optional. (Does not remove optional types inside collections.)
    /// </summary>
    public abstract NonOptionalType ToNonOptional();
}
