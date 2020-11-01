using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal class ShareExpression : Expression, IShareExpression
    {
        public BindingSymbol ReferencedSymbol { get; }
        public IExpression Referent { get; }

        public ShareExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            BindingSymbol referencedSymbol,
            IExpression referent)
            : base(span, dataType, semantics)
        {
            ReferencedSymbol = referencedSymbol;
            Referent = referent;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return $"⟦share⟧ {Referent}";
        }
    }
}
