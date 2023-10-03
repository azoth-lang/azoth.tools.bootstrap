using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

public record class ExpressionResult(IExpressionSyntax Syntax, ResultVariable Result)
{
    public DataType Type => Syntax.ConvertedDataType.Assigned();

    public void Deconstruct(out DataType type, out ResultVariable result)
    {
        type = Type;
        result = Result;
    }
}
