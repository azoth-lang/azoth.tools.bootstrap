using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface IVariableNameExpressionSyntax<TSymbol>
    where TSymbol : Symbol
{
    TextSpan Span { get; }
    Promise<TSymbol?> ReferencedSymbol { get; }
}

public partial interface ISimpleNameExpressionSyntax : IVariableNameExpressionSyntax<Symbol>
{
    new TextSpan Span { get; }
}

public partial interface ISelfExpressionSyntax : IVariableNameExpressionSyntax<SelfParameterSymbol>
{
    new TextSpan Span { get; }
}
