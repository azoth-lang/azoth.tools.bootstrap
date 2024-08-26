using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;

[Closed(typeof(SymbolTypeModel), typeof(CollectionTypeModel))]
public abstract class NonOptionalTypeModel : TypeModel
{
    protected NonOptionalTypeModel(Symbol underlyingSymbol)
        : base(underlyingSymbol) { }

    public sealed override NonOptionalTypeModel ToNonOptional() => this;

    public sealed override OptionalTypeModel ToOptional() => new(this);
}
