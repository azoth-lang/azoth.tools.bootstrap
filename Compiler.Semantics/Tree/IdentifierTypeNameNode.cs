using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdentifierTypeNameNode : TypeNameNode, IIdentifierTypeNameNode
{
    public override IIdentifierTypeNameSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;
    private ValueAttribute<ITypeSymbolNode?> referencedSymbolNode;
    public ITypeSymbolNode? ReferencedSymbolNode
        => referencedSymbolNode.TryGetValue(out var value) ? value
            : referencedSymbolNode.GetValue(this, SymbolNodeAttributes.StandardTypeName);
    public override TypeSymbol? ReferencedSymbol
        => SymbolAttribute.IdentifierTypeName(this);

    private ValueAttribute<BareType?> bareType;
    public override BareType? BareType
        => bareType.TryGetValue(out var value) ? value
            : bareType.GetValue(this, BareTypeAttribute.IdentifierTypeName);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeAttribute.IdentifierTypeName);

    public IdentifierTypeNameNode(IIdentifierTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
