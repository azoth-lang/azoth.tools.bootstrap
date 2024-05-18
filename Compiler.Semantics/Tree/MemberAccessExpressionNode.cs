using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MemberAccessExpressionNode : NameExpressionNode, IMemberAccessExpressionNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> context;
    public IAmbiguousExpressionNode Context => context.Value;
    public StandardName MemberName => Syntax.MemberName;
    public IFixedList<ITypeNode> TypeArguments { get; }

    public MemberAccessExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IAmbiguousExpressionNode context,
        IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
    }

    protected override IAssignableExpressionNode? Rewrite()
        => throw Child.RewriteNotSupported(this);

    public override ConditionalLexicalScope GetFlowLexicalScope() => Context.GetFlowLexicalScope();
}
