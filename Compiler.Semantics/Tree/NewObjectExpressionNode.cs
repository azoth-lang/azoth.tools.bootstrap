using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NewObjectExpressionNode : ExpressionNode, INewObjectExpressionNode
{
    public override INewObjectExpressionSyntax Syntax { get; }
    public ITypeNameNode ConstructingType { get; }
    public IdentifierName? ConstructorName => Syntax.ConstructorName;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public ConstructorSymbol? ReferencedSymbol => throw new NotImplementedException();

    public NewObjectExpressionNode(
        INewObjectExpressionSyntax syntax,
        ITypeNameNode type,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        ConstructingType = Child.Attach(this, type);
        Arguments = ChildList.Create(this, arguments);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
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

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (Arguments.IndexOf(child) is int argumentIndex)
            if (argumentIndex == 0)
                return base.InheritedPredecessor(child, descendant);
            else
                return (IFlowNode)Arguments[argumentIndex - 1];

        return base.InheritedPredecessor(child, descendant);
    }

    public override IFlowNode Predecessor()
        => (IFlowNode?)Arguments.LastOrDefault() ?? InheritedPredecessor();
}
