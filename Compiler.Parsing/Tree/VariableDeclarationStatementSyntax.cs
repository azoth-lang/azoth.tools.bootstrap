using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class VariableDeclarationStatementSyntax : StatementSyntax, IVariableDeclarationStatementSyntax
    {
        public bool IsMutableBinding { [DebuggerStepThrough] get; }
        public Name Name { get; }
        public Promise<int?> DeclarationNumber { get; } = new Promise<int?>();
        public Promise<VariableSymbol> Symbol { get; } = new Promise<VariableSymbol>();
        IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
        IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
        public TextSpan NameSpan { [DebuggerStepThrough] get; }
        public ITypeSyntax? Type { [DebuggerStepThrough] get; }
        public bool InferMutableType { [DebuggerStepThrough] get; }
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification =
            "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax? initializer;
        [DisallowNull]
        public ref IExpressionSyntax? Initializer
        {
            [DebuggerStepThrough]
            get => ref initializer;
        }

        public VariableDeclarationStatementSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name name,
            TextSpan nameSpan,
            ITypeSyntax? typeSyntax,
            bool inferMutableType,
            IExpressionSyntax? initializer)
            : base(span)
        {
            IsMutableBinding = isMutableBinding;
            Name = name;
            NameSpan = nameSpan;
            Type = typeSyntax;
            InferMutableType = inferMutableType;
            this.initializer = initializer;
        }

        public override string ToString()
        {
            var binding = IsMutableBinding ? "var" : "let";
            var type = Type != null ? ": " + Type : "";
            var initializer = Initializer != null ? " = " + Initializer : "";
            return $"{binding} {Name}{type}{initializer};";
        }
    }
}
