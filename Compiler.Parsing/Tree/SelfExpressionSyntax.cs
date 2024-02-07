using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class SelfExpressionSyntax : ExpressionSyntax, ISelfExpressionSyntax
{
    public bool IsImplicit { get; }
    public Promise<SelfParameterSymbol?> ReferencedSymbol { get; } = new Promise<SelfParameterSymbol?>();
    IPromise<Symbol?> IVariableNameExpressionSyntax.ReferencedSymbol => ReferencedSymbol;

    private Pseudotype? pseudotype;
    [DisallowNull]
    public Pseudotype? Pseudotype
    {
        [DebuggerStepThrough]
        get => pseudotype;
        set
        {
            if (pseudotype is not null) throw new InvalidOperationException("Can't set pseudotype repeatedly");
            pseudotype = value ?? throw new ArgumentNullException(nameof(Pseudotype), "Can't set Pseudotype to null");
        }
    }

    public SelfExpressionSyntax(TextSpan span, bool isImplicit)
        : base(span)
    {
        IsImplicit = isImplicit;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => IsImplicit ? "⟦self⟧" : "self";
}
