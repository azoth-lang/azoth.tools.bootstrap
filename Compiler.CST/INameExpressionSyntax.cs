using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface INameExpressionSyntax<TSymbol>
{
    TextSpan Span { get; }
    Promise<TSymbol?> ReferencedSymbol { get; }
}

public partial interface IStandardNameExpressionSyntax : INameExpressionSyntax<Symbol>
{
    new TextSpan Span { get; }
    //new Promise<Symbol?> ReferencedSymbol { get; }
}

public partial interface ISelfExpressionSyntax : INameExpressionSyntax<SelfParameterSymbol>
{
    new TextSpan Span { get; }
}
