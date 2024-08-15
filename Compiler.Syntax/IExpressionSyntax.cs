using Azoth.Tools.Bootstrap.Compiler.Core.Operators;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface IExpressionSyntax
{
    string ToGroupedString(OperatorPrecedence surroundingPrecedence);
}
