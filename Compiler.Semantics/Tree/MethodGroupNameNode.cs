using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodGroupNameNode : NameExpressionNode, IMethodGroupNameNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public StandardName MethodName { get; }
    public IFixedList<ITypeNode> TypeArguments { get; }
    public IFixedSet<IStandardMethodDeclarationNode> ReferencedDeclarations { get; }
    public override IFlowState FlowStateAfter => Context.FlowStateAfter;
    public IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations
        => GrammarAttribute.IsCached(in compatibleDeclarationsCached) ? compatibleDeclarations!
            : this.Synthetic(ref compatibleDeclarationsCached, ref compatibleDeclarations,
                OverloadResolutionAspect.MethodGroupName_CompatibleDeclarations);
    private IFixedSet<IStandardMethodDeclarationNode>? compatibleDeclarations;
    private bool compatibleDeclarationsCached;
    public IStandardMethodDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                OverloadResolutionAspect.MethodGroupName_ReferencedDeclaration);
    private IStandardMethodDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public override IMaybeExpressionAntetype Antetype => IAntetype.Unknown;

    public MethodGroupNameNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName methodName,
        IEnumerable<ITypeNode> typeArguments,
        IEnumerable<IStandardMethodDeclarationNode> referencedDeclarations)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        MethodName = methodName;
        TypeArguments = ChildList.Attach(this, typeArguments);
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.MethodGroupName_ControlFlowNext(this);

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
    {
        if (child == descendant) return null;
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child == descendant) return null;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder builder)
    {
        ExpressionTypesAspect.Expression_Contribute_Diagnostics(this, builder);
        OverloadResolutionAspect.MethodGroupName_Contribute_Diagnostics(this, builder);
        BindingAmbiguousNamesAspect.MethodGroupName_Contribute_Diagnostics(this, builder);
    }

    protected override IChildTreeNode Rewrite()
        => BindingAmbiguousNamesAspect.MethodGroupName_Rewrite_ToMethodName(this)
            ?? base.Rewrite();
}
