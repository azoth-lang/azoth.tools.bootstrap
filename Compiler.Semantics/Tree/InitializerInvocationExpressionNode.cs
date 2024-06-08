using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InitializerInvocationExpressionNode : ExpressionNode, IInitializerInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IInitializerGroupNameNode InitializerGroup { get; }
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    private ValueAttribute<IFixedSet<IInitializerDeclarationNode>> compatibleDeclarations;
    public IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations
        => compatibleDeclarations.TryGetValue(out var value) ? value
            : compatibleDeclarations.GetValue(this, OverloadResolutionAspect.InitializerInvocationExpression_CompatibleDeclarations);
    private ValueAttribute<IInitializerDeclarationNode?> referencedDeclaration;
    public IInitializerDeclarationNode? ReferencedDeclaration
        => referencedDeclaration.TryGetValue(out var value) ? value
            : referencedDeclaration.GetValue(this, OverloadResolutionAspect.InitializerInvocationExpression_ReferencedDeclaration);
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.InitializerInvocationExpression_Antetype);

    public InitializerInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IInitializerGroupNameNode initializerGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        InitializerGroup = Child.Attach(this, initializerGroup);
        Arguments = ChildList.Create(this, arguments);
    }
}
