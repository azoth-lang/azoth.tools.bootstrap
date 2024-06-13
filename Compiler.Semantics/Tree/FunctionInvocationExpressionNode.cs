using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionInvocationExpressionNode : ExpressionNode, IFunctionInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    private Child<IFunctionGroupNameNode> functionGroup;
    public IFunctionGroupNameNode FunctionGroup => functionGroup.Value;
    private readonly ChildList<IAmbiguousExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IEnumerable<IAmbiguousExpressionNode> IntermediateArguments => arguments.Final;
    public IEnumerable<IExpressionNode> FinalArguments => arguments.Final.Cast<IExpressionNode>();
    private ValueAttribute<IFixedSet<IFunctionLikeDeclarationNode>> compatibleDeclarations;
    public IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations
        => compatibleDeclarations.TryGetValue(out var value) ? value
            : compatibleDeclarations.GetValue(this, OverloadResolutionAspect.FunctionInvocationExpression_CompatibleDeclarations);
    private ValueAttribute<IFunctionLikeDeclarationNode?> referencedDeclaration;
    public IFunctionLikeDeclarationNode? ReferencedDeclaration
        => referencedDeclaration.TryGetValue(out var value) ? value
            : referencedDeclaration.GetValue(this, OverloadResolutionAspect.FunctionInvocationExpression_ReferencedDeclaration);
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.FunctionInvocationExpression_Antetype);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, ExpressionTypesAspect.FunctionInvocationExpression_Type);
    private ValueAttribute<ContextualizedOverload<IFunctionLikeDeclarationNode>?> contextualizedOverload;
    public ContextualizedOverload<IFunctionLikeDeclarationNode>? ContextualizedOverload
        => contextualizedOverload.TryGetValue(out var value) ? value
            : contextualizedOverload.GetValue(this, ExpressionTypesAspect.FunctionInvocationExpression_ContextualizedOverload);
    private ValueAttribute<FlowState> flowStateAfter;
    public override FlowState FlowStateAfter
        => flowStateAfter.TryGetValue(out var value) ? value
            : flowStateAfter.GetValue(this, ExpressionTypesAspect.FunctionInvocationExpression_FlowStateAfter);

    public FunctionInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IFunctionGroupNameNode functionGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.functionGroup = Child.Create(this, functionGroup);
        this.arguments = ChildList.Create(this, arguments);
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

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        OverloadResolutionAspect.FunctionInvocationExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    internal override FlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && arguments.IndexOfCurrent(ambiguousExpression) is int index and > 0)
            return ((IExpressionNode)arguments.FinalAt(index - 1)).FlowStateAfter;

        return base.InheritedFlowStateBefore(child, descendant);
    }

    public FlowState FlowStateBefore() => InheritedFlowStateBefore();
}
