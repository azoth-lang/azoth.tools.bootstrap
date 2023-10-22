using Azoth.Tools.Bootstrap.Compiler.Core.Operators;

namespace Azoth.Tools.Bootstrap.Compiler.AST;

public partial interface IExpression
{
    string ToGroupedString(OperatorPrecedence surroundingPrecedence);
}
