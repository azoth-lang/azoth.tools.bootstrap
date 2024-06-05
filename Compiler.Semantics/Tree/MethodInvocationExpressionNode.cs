using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodInvocationExpressionNode : ExpressionNode, IMethodInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IMethodGroupNameNode MethodGroup { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations
        => throw new System.NotImplementedException();
    public IStandardMethodDeclarationNode? ReferencedDeclaration
        => throw new System.NotImplementedException();

    public MethodInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IMethodGroupNameNode methodGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        MethodGroup = Child.Attach(this, methodGroup);
        Arguments = ChildList.Create(this, arguments);
    }
}
