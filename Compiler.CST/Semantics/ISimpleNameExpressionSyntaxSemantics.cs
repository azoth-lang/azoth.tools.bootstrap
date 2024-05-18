using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

[Closed(typeof(IIdentifierNameExpressionSyntaxSemantics), typeof(ISelfExpressionSyntaxSemantics))]
public interface ISimpleNameExpressionSyntaxSemantics : ISyntaxSemantics
{
    new IPromise<DataType> Type { get; }
}
