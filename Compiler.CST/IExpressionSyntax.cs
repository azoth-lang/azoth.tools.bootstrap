using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public partial interface IExpressionSyntax
{
    string ToGroupedString(OperatorPrecedence surroundingPrecedence);
    void AddConversion(ChainedConversion conversion);
}
