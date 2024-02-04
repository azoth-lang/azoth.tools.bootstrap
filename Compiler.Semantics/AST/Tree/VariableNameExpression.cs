using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class VariableNameExpression : Expression, IVariableNameExpression
{
    public NamedBindingSymbol ReferencedSymbol { get; }
    public Promise<bool> VariableIsLiveAfter { get; } = new Promise<bool>();

    public VariableNameExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        NamedBindingSymbol referencedSymbol)
        : base(span, dataType, semantics)
    {
        ReferencedSymbol = referencedSymbol;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => ReferencedSymbol.Name.ToString();
}