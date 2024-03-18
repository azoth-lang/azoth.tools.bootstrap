using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class InvocationExpressionSyntax : DataTypedExpressionSyntax, IInvocationExpressionSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }
    public Promise<Symbol?> ReferencedSymbol { get; } = new Promise<Symbol?>();

    private LexicalScope? containingLexicalScope;
    public LexicalScope ContainingLexicalScope
    {
        [DebuggerStepThrough]
        get =>
            containingLexicalScope
            ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
        [DebuggerStepThrough]
        set
        {
            if (containingLexicalScope is not null)
                throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
            containingLexicalScope = value;
        }
    }

    public InvocationExpressionSyntax(
        TextSpan span,
        IExpressionSyntax expression,
        IFixedList<IExpressionSyntax> arguments)
        : base(span)
    {
        Expression = expression;
        Arguments = arguments;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"{Expression}({string.Join(", ", Arguments)})";
}
