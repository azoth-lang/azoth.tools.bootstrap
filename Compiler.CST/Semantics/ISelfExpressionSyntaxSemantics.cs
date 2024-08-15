using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

[Closed(typeof(UnknownNameSyntax))]
public interface ISelfExpressionSyntaxSemantics : ISimpleNameExpressionSyntaxSemantics
{
    IPromise<Pseudotype> Pseudotype { get; }
}
