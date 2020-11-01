using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitMutateExpressionSyntax : ImplicitExpressionSyntax, IMutateExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification =
            "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;

        public ref IExpressionSyntax Referent
        {
            [DebuggerStepThrough]
            get => ref referent;
        }
        public Promise<BindingSymbol?> ReferencedSymbol { get; }

        public ImplicitMutateExpressionSyntax(IExpressionSyntax referent, DataType type, BindingSymbol? referencedSymbol)
            : base(type, referent.Span, ExpressionSemantics.Borrow)
        {
            this.referent = referent;
            ReferencedSymbol = Promise.ForValue(referencedSymbol);
        }

        public override string ToString()
        {
            return $"⟦borrow⟧ {Referent}";
        }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return $"({this})";
        }
    }
}
