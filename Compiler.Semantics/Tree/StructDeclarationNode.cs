using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StructDeclarationNode : TypeDeclarationNode, IStructDeclarationNode
{
    public override IStructDeclarationSyntax Syntax { get; }

    private ValueAttribute<IStructSymbolNode> symbolNode;
    public override IStructSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value)
            ? value
            : symbolNode.GetValue(this, SymbolNodeAttribute.StructDeclaration);
    public override IFixedList<IStructMemberDeclarationNode> Members { get; }

    public StructDeclarationNode(
        IStructDeclarationSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<ISupertypeNameNode> supertypeNames,
        IEnumerable<IStructMemberDeclarationNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildList.CreateFixed(this, members);
    }
}
