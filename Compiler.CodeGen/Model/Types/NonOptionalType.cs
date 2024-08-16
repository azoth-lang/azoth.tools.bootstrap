using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[Closed(typeof(SymbolType), typeof(CollectionType))]
public abstract class NonOptionalType : TypeModel
{
    protected NonOptionalType(Symbol underlyingSymbol)
        : base(underlyingSymbol) { }

    public sealed override NonOptionalType ToNonOptional() => this;
}
