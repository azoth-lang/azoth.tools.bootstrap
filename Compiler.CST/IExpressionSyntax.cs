using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IExpressionSyntax
{
    [DisallowNull] DataType? DataType { get; set; }
    [DisallowNull] ExpressionSemantics? Semantics { get; set; }
    string ToGroupedString(OperatorPrecedence surroundingPrecedence);
    void AddConversion(ChainedConversion conversion);
}
