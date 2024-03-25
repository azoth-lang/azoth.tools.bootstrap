using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class SelfExpressionSyntax : NameExpressionSyntax, ISelfExpressionSyntax
{
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public override Promise<ISelfExpressionSyntaxSemantics> Semantics { [DebuggerStepThrough] get; } = new();
    IPromise<IVariableNameExpressionSyntaxSemantics> IVariableNameExpressionSyntax.Semantics => Semantics;
    public override IPromise<SelfParameterSymbol?> ReferencedSymbol { [DebuggerStepThrough] get; }
    IPromise<Symbol?> INameExpressionSyntax.ReferencedSymbol => ReferencedSymbol;
    IPromise<SelfParameterSymbol?> INameExpressionSyntax<SelfParameterSymbol>.ReferencedSymbol
        => ReferencedSymbol;
    public override IPromise<DataType> DataType { [DebuggerStepThrough] get; }
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
    public IPromise<Pseudotype> Pseudotype { get; }

    public SelfExpressionSyntax(TextSpan span, bool isImplicit)
        : base(span)
    {
        IsImplicit = isImplicit;
        ReferencedSymbol = Semantics.Select(s => s.Symbol);
        DataType = Semantics.Select(s => s.Type).Flatten();
        Pseudotype = Semantics.Select(s => s.Pseudotype).Flatten();
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => IsImplicit ? "⟦self⟧" : "self";
}
