using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodInvocationExpressionNode : ExpressionNode, IMethodInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IMethodGroupNameNode MethodGroup { get; }
    private readonly ChildList<IAmbiguousExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IEnumerable<IAmbiguousExpressionNode> IntermediateArguments => arguments.Final;
    private ValueAttribute<IFixedSet<IStandardMethodDeclarationNode>> compatibleDeclarations;
    public IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations
        => compatibleDeclarations.TryGetValue(out var value) ? value
            : compatibleDeclarations.GetValue(this, OverloadResolutionAspect.MethodInvocationExpression_CompatibleDeclarations);
    private ValueAttribute<IStandardMethodDeclarationNode?> referencedDeclaration;
    public IStandardMethodDeclarationNode? ReferencedDeclaration
        => referencedDeclaration.TryGetValue(out var value) ? value
            : referencedDeclaration.GetValue(this, OverloadResolutionAspect.MethodInvocationExpression_ReferencedDeclaration);
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.MethodInvocationExpression_Antetype);

    public MethodInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IMethodGroupNameNode methodGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        MethodGroup = Child.Attach(this, methodGroup);
        this.arguments = ChildList.Create(this, arguments);
    }
}
