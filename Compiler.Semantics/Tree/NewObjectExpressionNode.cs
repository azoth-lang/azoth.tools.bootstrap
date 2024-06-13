using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NewObjectExpressionNode : ExpressionNode, INewObjectExpressionNode
{
    public override INewObjectExpressionSyntax Syntax { get; }
    public ITypeNameNode ConstructingType { get; }
    public IdentifierName? ConstructorName => Syntax.ConstructorName;
    private readonly ChildList<IAmbiguousExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IEnumerable<IAmbiguousExpressionNode> IntermediateArguments => arguments.Final;
    public IEnumerable<IExpressionNode> FinalArguments => arguments.Final.Cast<IExpressionNode>();
    private ValueAttribute<IMaybeAntetype> constructingAntetype;
    public IMaybeAntetype ConstructingAntetype
        => constructingAntetype.TryGetValue(out var value) ? value
            : constructingAntetype.GetValue(this, NameBindingAntetypesAspect.NewObjectExpression_ConstructingAntetype);
    private ValueAttribute<IFixedSet<IConstructorDeclarationNode>> referencedConstructors;
    public IFixedSet<IConstructorDeclarationNode> ReferencedConstructors
        => referencedConstructors.TryGetValue(out var value) ? value
            : referencedConstructors.GetValue(this, BindingNamesAspect.NewObjectExpression_ReferencedConstructors);
    private ValueAttribute<IFixedSet<IConstructorDeclarationNode>> compatibleConstructors;
    public IFixedSet<IConstructorDeclarationNode> CompatibleConstructors
        => compatibleConstructors.TryGetValue(out var value) ? value
            : compatibleConstructors.GetValue(this, OverloadResolutionAspect.NewObjectExpression_CompatibleConstructors);
    private ValueAttribute<IConstructorDeclarationNode?> referencedConstructor;
    public IConstructorDeclarationNode? ReferencedConstructor
        => referencedConstructor.TryGetValue(out var value) ? value
            : referencedConstructor.GetValue(this, OverloadResolutionAspect.NewObjectExpression_ReferencedConstructor);
    private ValueAttribute<ContextualizedOverload<IConstructorDeclarationNode>?> contextualizedOverload;
    public ContextualizedOverload<IConstructorDeclarationNode>? ContextualizedOverload
        => contextualizedOverload.TryGetValue(out var value) ? value
            : contextualizedOverload.GetValue(this, ExpressionTypesAspect.NewObjectExpression_ContextualizedOverload);
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.NewObjectExpression_Antetype);
    private ValueAttribute<FlowState> flowStateAfter;
    public override FlowState FlowStateAfter
        => flowStateAfter.TryGetValue(out var value) ? value
            : flowStateAfter.GetValue(this, ExpressionTypesAspect.NewObjectExpression_FlowStateAfter);
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, ExpressionTypesAspect.NewObjectExpression_Type);

    public NewObjectExpressionNode(
        INewObjectExpressionSyntax syntax,
        ITypeNameNode type,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        ConstructingType = Child.Attach(this, type);
        this.arguments = ChildList.Create(this, arguments);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        OverloadResolutionAspect.NewObjectExpression_ContributeDiagnostics(this, diagnostics);
        ExpressionTypesAspect.NewObjectExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == ConstructingType)
            return GetContainingLexicalScope();
        var argumentIndex = Arguments.IndexOf(child)
                            ?? throw new ArgumentException("Is not a child of this node.", nameof(child));
        if (argumentIndex == 0)
            return GetContainingLexicalScope();

        return Arguments[argumentIndex - 1].GetFlowLexicalScope().True;
    }

    public new PackageNameScope InheritedPackageNameScope() => base.InheritedPackageNameScope();

    public FlowState FlowStateBefore() => InheritedFlowStateBefore();

    internal override FlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && arguments.IndexOfCurrent(ambiguousExpression) is int index and > 0)
            return ((IExpressionNode)arguments.FinalAt(index - 1)).FlowStateAfter;
        return base.InheritedFlowStateBefore(child, descendant);
    }
}
