using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InitializerGroupNameNode : AmbiguousNameExpressionNode, IInitializerGroupNameNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    public ITypeNameExpressionNode? Context { get; }
    public StandardName InitializerName => Syntax.MemberName;
    public IFixedSet<IInitializerDeclarationNode> ReferencedDeclarations { get; }

    public InitializerGroupNameNode(
        IMemberAccessExpressionSyntax syntax,
        ITypeNameExpressionNode? context,
        IEnumerable<IInitializerDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        Context = Child.Attach(this, context);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }
}
