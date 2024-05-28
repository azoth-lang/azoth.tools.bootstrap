using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExpressionBodyNode : CodeNode, IExpressionBodyNode
{
    public override IExpressionBodySyntax Syntax { get; }
    public IResultStatementNode ResultStatement { get; }
    private readonly IFixedList<IStatementNode> statements;
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => statements;
    private ValueAttribute<ValueIdScope> valueIdScope;
    public ValueIdScope ValueIdScope
        => valueIdScope.TryGetValue(out var value) ? value
            : valueIdScope.GetValue(this, TypeMemberDeclarationsAspect.Body_ValueIdScope);

    public ExpressionBodyNode(IExpressionBodySyntax syntax, IResultStatementNode resultStatement)
    {
        Syntax = syntax;
        ResultStatement = Child.Attach(this, resultStatement);
        statements = FixedList.Create(ResultStatement);
    }

    public LexicalScope GetContainingLexicalScope() => InheritedContainingLexicalScope();

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        var statementIndex = child == ResultStatement ? 0 : throw new InvalidOperationException("Caller is not a child");
        return LexicalScopingAspect.BodyOrBlock_InheritedLexicalScope(this, statementIndex);
    }

    public IParameterNode? Predecessor() => (IParameterNode?)InheritedPredecessor();

    internal override ValueIdScope InheritedValueIdScope(IChildNode child, IChildNode descendant)
        => ValueIdScope;

    internal override ISemanticNode? InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == ResultStatement)
            // The result statement is the first expression in the body.
            return null;
        return base.InheritedPredecessor(child, descendant);
    }
}
