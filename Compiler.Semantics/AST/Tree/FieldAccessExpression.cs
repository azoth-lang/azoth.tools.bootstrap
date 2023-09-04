using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class FieldAccessExpression : Expression, IFieldAccessExpression
{
    public IExpression Context { get; }
    public AccessOperator AccessOperator { get; }
    public FieldSymbol ReferencedSymbol { get; }

    public FieldAccessExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IExpression context,
        AccessOperator accessOperator,
        FieldSymbol referencedSymbol)
        : base(span, dataType, semantics)
    {
        Context = context;
        AccessOperator = accessOperator;
        ReferencedSymbol = referencedSymbol;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{Context.ToGroupedString(ExpressionPrecedence)}{AccessOperator.ToSymbolString()}{ReferencedSymbol.Name}";
}
