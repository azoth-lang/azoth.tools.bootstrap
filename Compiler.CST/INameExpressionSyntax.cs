using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface INameExpressionSyntax<out TSymbol>
{
    TextSpan Span { get; }
    IPromise<TSymbol?> ReferencedSymbol { get; }
}

public partial interface IStandardNameExpressionSyntax : INameExpressionSyntax<Symbol>
{
    new TextSpan Span { get; }
    new IPromise<Symbol?> ReferencedSymbol { get; }
}

public partial interface ISelfExpressionSyntax : INameExpressionSyntax<SelfParameterSymbol>
{
    new TextSpan Span { get; }
}
