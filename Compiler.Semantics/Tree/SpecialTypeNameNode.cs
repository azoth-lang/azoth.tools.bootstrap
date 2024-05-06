using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SpecialTypeNameNode : TypeNameNode, ISpecialTypeNameNode
{
    public override ISpecialTypeNameSyntax Syntax { get; }
    public override SpecialTypeName Name => Syntax.Name;
    private ValueAttribute<TypeSymbol> referencedSymbol;
    public override TypeSymbol ReferencedSymbol
        => referencedSymbol.TryGetValue(out var value) ? value
            : referencedSymbol.GetValue(this, SymbolAttribute.SpecialTypeName);

    public SpecialTypeNameNode(ISpecialTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
