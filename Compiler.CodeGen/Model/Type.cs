using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Type
{
    public Language Language { get; }
    public Grammar Grammar { get; }
    public TypeNode Syntax { get; }


    public Symbol Symbol { get; }
    public string Name => Symbol.Name;
    public bool IsList => Syntax.IsList;
    public bool IsOptional => Syntax.IsOptional;

    public Type(Property property, TypeNode syntax)
    {
        Language = property.Rule.Grammar.Language;
        Grammar = property.Rule.Grammar;
        Syntax = syntax;
        Symbol = new Symbol(property.Rule.Grammar, syntax.Symbol);
    }

    public bool IsEquivalentTo(Type other)
    {
        return IsList == other.IsList
            && IsOptional == other.IsOptional
            && Name == other.Name
            && Symbol.Syntax == other.Symbol.Syntax;
    }
}
