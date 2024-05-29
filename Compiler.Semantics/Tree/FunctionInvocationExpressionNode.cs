using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionInvocationExpressionNode : ExpressionNode, IFunctionInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IFunctionGroupNameNode Function { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }

    public FunctionInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IFunctionGroupNameNode function,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        Function = Child.Attach(this, function);
        Arguments = ChildList.Create(this, arguments);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (Arguments.IndexOf(child) is int argumentIndex)
        {
            if (argumentIndex == 0) return GetContainingLexicalScope();

            return Arguments[argumentIndex - 1].GetFlowLexicalScope().True;
        }

        return base.InheritedContainingLexicalScope(child, descendant);
    }
}
