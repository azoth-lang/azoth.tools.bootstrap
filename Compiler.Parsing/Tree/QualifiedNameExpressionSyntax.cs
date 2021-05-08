using System.Diagnostics;
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
        public IExpressionSyntax Context { [DebuggerStepThrough] get; }
        public AccessOperator AccessOperator { [DebuggerStepThrough] get; }
        public INameExpressionSyntax Field { [DebuggerStepThrough] get; }
        public IPromise<FieldSymbol?> ReferencedSymbol => Field.ReferencedSymbol.Select(s => (FieldSymbol?)s);

        public QualifiedNameExpressionSyntax(
            TextSpan span,
            IExpressionSyntax context,
            AccessOperator accessOperator,
            INameExpressionSyntax field)
            : base(span)
        {
            Context = context;
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
