using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InitializerInvocationExpressionNode : ExpressionNode, IInitializerInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IInitializerGroupNameNode InitializerGroup { get; }
    private readonly IRewritableChildList<IAmbiguousExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
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
    private ValueAttribute<ContextualizedOverload?> contextualizedOverload;
    public ContextualizedOverload? ContextualizedOverload
        => contextualizedOverload.TryGetValue(out var value) ? value
            : contextualizedOverload.GetValue(this, ExpressionTypesAspect.InitializerInvocationExpression_ContextualizedOverload);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.InitializerInvocationExpression_Type);

    public InitializerInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IInitializerGroupNameNode initializerGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        InitializerGroup = Child.Attach(this, initializerGroup);
        this.arguments = ChildList.Create(this, nameof(Arguments), arguments);
    }
}
