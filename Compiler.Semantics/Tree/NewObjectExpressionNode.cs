using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NewObjectExpressionNode : ExpressionNode, INewObjectExpressionNode
{
    public override INewObjectExpressionSyntax Syntax { get; }
    public ITypeNameNode Type { get; }
    public IdentifierName? ConstructorName => Syntax.ConstructorName;
    public IFixedList<IUntypedExpressionNode> Arguments { get; }
    public ConstructorSymbol? ReferencedSymbol => throw new NotImplementedException();

    public NewObjectExpressionNode(
        INewObjectExpressionSyntax syntax,
        ITypeNameNode type,
        IEnumerable<IUntypedExpressionNode> arguments)
    {
        Syntax = syntax;
        Type = Child.Attach(this, type);
        Arguments = ChildList.Create(this, arguments);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        ExpressionTypesAspect.NewObjectExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Type)
            return GetContainingLexicalScope();
        var argumentIndex = Arguments.IndexOf(child)
                            ?? throw new ArgumentException("Is not a child of this node.", nameof(child));
        if (argumentIndex == 0)
            return GetContainingLexicalScope();

        return Arguments[argumentIndex - 1].GetFlowLexicalScope().True;
    }
}
