using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

[Closed(typeof(SelfExpressionSyntax), typeof(UnknownNameSyntax))]
public interface ISelfExpressionSyntaxSemantics : IVariableNameExpressionSyntaxSemantics
{
    SelfParameterSymbol? Symbol { get; }
    IPromise<Pseudotype> Pseudotype { get; }
}
