using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MemberAccessExpressionNode : AmbiguousNameExpressionNode, IMemberAccessExpressionNode
{
    protected override bool MayHaveRewrite => true;

    public override IMemberAccessExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> context;
    private bool contextCached;
    public IAmbiguousExpressionNode TempContext
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode? Context => TempContext as IExpressionNode;
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

    protected override IChildNode Rewrite()
        => BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_FunctionOrMethodGroupNameContext(this)
        ?? BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_NamespaceNameContext(this)
        ?? BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_TypeNameExpressionContext(this)
        ?? BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_ExpressionContext(this)
        ?? BindingAmbiguousNamesAspect.MemberAccessExpression_Rewrite_UnknownNameExpressionContext(this)
        ?? base.Rewrite();

    public override ConditionalLexicalScope FlowLexicalScope() => TempContext.FlowLexicalScope();

    public PackageNameScope PackageNameScope() => InheritedPackageNameScope();

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => null;

    internal override bool InheritedImplicitRecoveryAllowed(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        // By default, implicit recovery is not allowed
        => false;
}
