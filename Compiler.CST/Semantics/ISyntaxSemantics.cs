using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

[Closed(
    typeof(IMemberAccessSyntaxSemantics),
    typeof(ISimpleNameExpressionSyntaxSemantics))]
public interface ISyntaxSemantics
{
    IFixedSet<Symbol> Symbols { get; }
    IPromise<DataType?> Type { get; }
}
