using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodInvocationExpressionNode : ExpressionNode, IMethodInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IMethodGroupNameNode MethodGroup { get; }
    private readonly ChildList<IAmbiguousExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    // TODO Don't make this require every argument to be final
    public IEnumerable<IAmbiguousExpressionNode> IntermediateArguments => arguments.Final;
    // TODO Don't make this require every argument to be final
    public IEnumerable<IExpressionNode> FinalArguments => arguments.Final.Cast<IExpressionNode>();
    private ValueAttribute<IFixedSet<IStandardMethodDeclarationNode>> compatibleDeclarations;
    public IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations
        => compatibleDeclarations.TryGetValue(out var value) ? value
            : compatibleDeclarations.GetValue(this, OverloadResolutionAspect.MethodInvocationExpression_CompatibleDeclarations);
    private ValueAttribute<IStandardMethodDeclarationNode?> referencedDeclaration;
    public IStandardMethodDeclarationNode? ReferencedDeclaration
        => referencedDeclaration.TryGetValue(out var value) ? value
            : referencedDeclaration.GetValue(this, OverloadResolutionAspect.MethodInvocationExpression_ReferencedDeclaration);
    private ValueAttribute<ContextualizedOverload<IStandardMethodDeclarationNode>?> contextualizedOverload;
    public ContextualizedOverload<IStandardMethodDeclarationNode>? ContextualizedOverload
        => contextualizedOverload.TryGetValue(out var value) ? value
            : contextualizedOverload.GetValue(this, ExpressionTypesAspect.MethodInvocationExpression_ContextualizedOverload);
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.MethodInvocationExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.MethodInvocationExpression_Type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.MethodInvocationExpression_FlowStateAfter);

    public MethodInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IMethodGroupNameNode methodGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        MethodGroup = Child.Attach(this, methodGroup);
        this.arguments = ChildList.Create(this, arguments);
    }

    internal override FlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && arguments.IndexOfCurrent(ambiguousExpression) is int index)
        {
            if (index == 0)
                return MethodGroup.FlowStateAfter;
            return ((IExpressionNode)arguments.FinalAt(index - 1)).FlowStateAfter;
        }
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }
}
