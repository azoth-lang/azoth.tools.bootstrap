using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal class NoneLiteralExpression : LiteralExpression, INoneLiteralExpression
    {
        public NoneLiteralExpression(TextSpan span, DataType dataType, ExpressionSemantics semantics)
            : base(span, dataType, semantics) { }

        public override string ToString() => "none";
    }
}
