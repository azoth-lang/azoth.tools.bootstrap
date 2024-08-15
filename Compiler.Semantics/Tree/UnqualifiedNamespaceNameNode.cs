using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UnqualifiedNamespaceNameNode : NamespaceNameNode, IUnqualifiedNamespaceNameNode
{
    public override IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName Name => Syntax.Name;
    public override IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; }

    public UnqualifiedNamespaceNameNode(
        IIdentifierNameExpressionSyntax syntax,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
    }
}
