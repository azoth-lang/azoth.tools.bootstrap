using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.AST
{
    public partial interface IExpression
    {
        string ToGroupedString(OperatorPrecedence surroundingPrecedence);
    }
}
