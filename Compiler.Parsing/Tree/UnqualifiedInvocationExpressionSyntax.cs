using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    internal class UnqualifiedInvocationExpressionSyntax : InvocationExpressionSyntax, IUnqualifiedInvocationExpressionSyntax
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

        public NamespaceName Namespace { get; }
        public new Promise<FunctionSymbol?> ReferencedSymbol { get; }

        public UnqualifiedInvocationExpressionSyntax(
            TextSpan span,
            Name invokedName,
            TextSpan invokedNameSpan,
            FixedList<IArgumentSyntax> arguments)
            : base(span, invokedName, invokedNameSpan, arguments, new Promise<FunctionSymbol?>())
        {
            Namespace = NamespaceName.Global;
            ReferencedSymbol = (Promise<FunctionSymbol?>)base.ReferencedSymbol;
        }

        public IEnumerable<IPromise<FunctionSymbol>> LookupInContainingScope()
        {
            if (containingLexicalScope == null)
                throw new InvalidOperationException($"Can't lookup function name without {nameof(ContainingLexicalScope)}");

            // If name is unknown, no symbols
            if (InvokedName is null) return Enumerable.Empty<IPromise<FunctionSymbol>>();

            return containingLexicalScope.Lookup(InvokedName).Select(p => p.As<FunctionSymbol>()).WhereNotNull();
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
        public override string ToString()
        {
            return $"{InvokedName}({string.Join(", ", Arguments)})";
        }
    }
}
