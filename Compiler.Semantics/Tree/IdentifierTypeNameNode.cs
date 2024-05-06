using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdentifierTypeNameNode : TypeNameNode, IIdentifierTypeNameNode
{
    public override IIdentifierTypeNameSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;
    private ValueAttribute<ITypeSymbolNode> referencedSymbolNode;
    public ITypeSymbolNode ReferencedSymbolNode
        => referencedSymbolNode.TryGetValue(out var value) ? value
            : referencedSymbolNode.GetValue(this, SymbolNodeAttribute.StandardTypeName);
    public override TypeSymbol ReferencedSymbol
        => SymbolAttribute.IdentifierTypeName(this);

    public IdentifierTypeNameNode(IIdentifierTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
