using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class QualifiedInvocationExpressionSyntax : InvocationExpressionSyntax, IQualifiedInvocationExpressionSyntax
    {
        private LexicalScope? containingLexicalScope;
        public LexicalScope ContainingLexicalScope
        {
            [DebuggerStepThrough]
            get =>
                containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            [DebuggerStepThrough]
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }

        private IExpressionSyntax context;
        public ref IExpressionSyntax Context => ref context;
        public new Promise<MethodSymbol?> ReferencedSymbol { get; }

        public QualifiedInvocationExpressionSyntax(
            TextSpan span,
            IExpressionSyntax context,
            Name invokedName,
            TextSpan invokedNameSpan,
            FixedList<IArgumentSyntax> arguments)
            : base(span, invokedName, invokedNameSpan, arguments, new Promise<MethodSymbol?>())
        {
            this.context = context;
            ReferencedSymbol = (Promise<MethodSymbol?>)base.ReferencedSymbol;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{Context.ToGroupedString(ExpressionPrecedence)}.{InvokedName}({string.Join(", ", Arguments)})";
        }
    }
}
