using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

[Closed(
    typeof(TypeNameSyntax),
    typeof(FunctionGroupNameSyntax),
    typeof(NamespaceNameSyntax),
    typeof(UnknownNameSyntax))]
public interface IIdentifierNameExpressionSyntaxSemantics : ISimpleNameExpressionSyntaxSemantics
{
    IPromise<Symbol?> Symbol { get; }
}
