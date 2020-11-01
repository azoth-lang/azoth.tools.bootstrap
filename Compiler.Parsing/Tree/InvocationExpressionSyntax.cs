using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal abstract class InvocationExpressionSyntax : ExpressionSyntax, IInvocationExpressionSyntax
    {
        public Name InvokedName { get; }
        public TextSpan InvokedNameSpan { get; }
        public FixedList<IArgumentSyntax> Arguments { [DebuggerStepThrough] get; }
        public IPromise<InvocableSymbol?> ReferencedSymbol { get; }

        private protected InvocationExpressionSyntax(
            TextSpan span,
            Name invokedName,
            TextSpan invokedNameSpan,
            FixedList<IArgumentSyntax> arguments,
            IPromise<InvocableSymbol?> referencedSymbol)
            : base(span)
        {
            InvokedName = invokedName;
            Arguments = arguments;
            ReferencedSymbol = referencedSymbol;
            InvokedNameSpan = invokedNameSpan;
        }
    }
}
