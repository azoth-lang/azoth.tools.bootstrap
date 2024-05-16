using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
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
}
