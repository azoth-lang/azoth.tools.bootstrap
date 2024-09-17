using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class QualifiedNamespaceNameNode : NamespaceNameNode, IQualifiedNamespaceNameNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    public INamespaceNameNode Context { get; }
    public INamespaceNameNode CurrentContext => Context;
    public IdentifierName Name => (IdentifierName)Syntax.MemberName;
    public override IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; }

    public QualifiedNamespaceNameNode(
        IMemberAccessExpressionSyntax syntax,
        INamespaceNameNode context,
        IEnumerable<INamespaceDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
    }
}
