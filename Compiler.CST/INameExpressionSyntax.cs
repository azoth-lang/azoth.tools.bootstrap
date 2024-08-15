using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface INameExpressionSyntax<out TSymbol>
{
    TextSpan Span { get; }
}

public partial interface IStandardNameExpressionSyntax : INameExpressionSyntax<Symbol>
{
    new TextSpan Span { get; }
}
