using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SpecialTypeNameExpressionNode : NameExpressionNode, ISpecialTypeNameExpressionNode
{
    public override ISpecialTypeNameExpressionSyntax Syntax { get; }
    public SpecialTypeName Name => Syntax.Name;
    public TypeSymbol? ReferencedSymbol => throw new NotImplementedException();

    public SpecialTypeNameExpressionNode(ISpecialTypeNameExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
