using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class QualifiedNameExpressionSyntax : ExpressionSyntax, IQualifiedNameExpressionSyntax
    {
        private IExpressionSyntax context;
        public ref IExpressionSyntax Context => ref context;

        public AccessOperator AccessOperator { get; }
        public INameExpressionSyntax Field { get; }
        public IPromise<FieldSymbol?> ReferencedSymbol => Field.ReferencedSymbol.Select(s => (FieldSymbol?)s);

        public QualifiedNameExpressionSyntax(
            TextSpan span,
            IExpressionSyntax context,
            AccessOperator accessOperator,
            INameExpressionSyntax field)
            : base(span)
        {
            this.context = context;
            AccessOperator = accessOperator;
            Field = field;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{Context.ToGroupedString(ExpressionPrecedence)}{AccessOperator.ToSymbolString()}{Field}";
        }
    }
}
