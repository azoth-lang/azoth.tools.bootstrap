using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MemberAccessExpressionNode : AmbiguousNameExpressionNode, IMemberAccessExpressionNode
{
    protected override bool MayHaveRewrite => true;

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

    protected override IAmbiguousNameExpressionNode? Rewrite()
        => BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_FunctionOrMethodGroupNameContext(this)
        ?? BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_NamespaceNameContext(this)
        ?? BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_TypeNameExpressionContext(this)
        ?? BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_ExpressionContext(this)
        ?? BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_UnknownNameExpressionContext(this);

    public override ConditionalLexicalScope GetFlowLexicalScope() => Context.GetFlowLexicalScope();

    public new PackageNameScope InheritedPackageNameScope() => base.InheritedPackageNameScope();
}
