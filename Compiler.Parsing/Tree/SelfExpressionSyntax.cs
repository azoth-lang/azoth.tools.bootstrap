using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class SelfExpressionSyntax : NameExpressionSyntax, ISelfExpressionSyntax
{
    public bool IsImplicit { get; }
    public override Promise<SelfParameterSymbol?> ReferencedSymbol { get; } = new Promise<SelfParameterSymbol?>();
    IPromise<Symbol?> INameExpressionSyntax.ReferencedSymbol => ReferencedSymbol;
    public override Promise<DataType> DataType { get; } = new();
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
    public Promise<Pseudotype> Pseudotype { get; } = new();

    public SelfExpressionSyntax(TextSpan span, bool isImplicit)
        : base(span)
    {
        IsImplicit = isImplicit;
    }

    public void FulfillDataType(DataType type) => DataType.Fulfill(type);

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => IsImplicit ? "⟦self⟧" : "self";
}
