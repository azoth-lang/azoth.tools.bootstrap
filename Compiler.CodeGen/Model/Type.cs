using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Type
{
    public static Type Void { get; } = new Type(null, Symbol.Void);

    public static Type? Create(Grammar? grammar, Symbol? symbol)
        => symbol is not null ? new Type(grammar, symbol) : null;

    public Language? Language { get; }
    public Grammar? Grammar { get; }
    public TypeNode? Syntax { get; }


    public Symbol Symbol { get; }
    public string Name => Symbol.Name;
    public CollectionKind CollectionKind => Syntax?.CollectionKind ?? CollectionKind.None;
    public bool IsOptional => Syntax?.IsOptional ?? false;

    public Type(Grammar? grammar, TypeNode syntax)
    {
        Language = grammar?.Language;
        Grammar = grammar;
        Syntax = syntax;
        Symbol = new Symbol(grammar, syntax.Symbol);
    }

    private Type(Grammar? grammar, Symbol symbol)
    {
        Language = grammar?.Language;
        Grammar = grammar;
        Symbol = symbol;
    }

    public bool IsEquivalentTo(Type other)
    {
        return CollectionKind == other.CollectionKind
               && IsOptional == other.IsOptional
               && Name == other.Name
               && Symbol.Syntax == other.Symbol.Syntax;
    }
}
