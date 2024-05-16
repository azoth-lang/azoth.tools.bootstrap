using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MemberAccessExpressionNode : NameExpressionNode, IMemberAccessExpressionNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> context;
    public IUntypedExpressionNode Context => context.Value;
    public AccessOperator AccessOperator => Syntax.AccessOperator;
    public StandardName MemberName => Syntax.MemberName;
    public IFixedList<ITypeNode> TypeArguments { get; }

    public MemberAccessExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IUntypedExpressionNode context,
        IEnumerable<ITypeNode> typeArguments)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        TypeArguments = ChildList.Attach(this, typeArguments);
    }

    protected override IAssignableExpressionNode Rewrite() => (IAssignableExpressionNode)base.Rewrite();
}
