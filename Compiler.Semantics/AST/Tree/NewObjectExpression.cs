using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class NewObjectExpression : Expression, INewObjectExpression
{
    public ConstructorSymbol ReferencedSymbol { get; }
    public IFixedList<IExpression> Arguments { get; }

    public NewObjectExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        ConstructorSymbol referencedSymbol,
        FixedList<IExpression> arguments)
        : base(span, dataType, semantics)
    {
        ReferencedSymbol = referencedSymbol;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString()
    {
        var name = ReferencedSymbol.Name is not null ? "." + ReferencedSymbol.Name : "";
        return $"new {ReferencedSymbol.ContainingSymbol}{name}({string.Join(", ", Arguments)})";
    }
}
