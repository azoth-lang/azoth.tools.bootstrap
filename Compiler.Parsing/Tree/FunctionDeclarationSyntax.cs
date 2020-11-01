using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class FunctionDeclarationSyntax : InvocableDeclarationSyntax, IFunctionDeclarationSyntax
    {
        public NamespaceName ContainingNamespaceName { get; }

        private NamespaceOrPackageSymbol? containingNamespaceSymbol;
        public NamespaceOrPackageSymbol ContainingNamespaceSymbol
        {
            get => containingNamespaceSymbol
                   ?? throw new InvalidOperationException($"{ContainingNamespaceSymbol} not yet assigned");
            set
            {
                if (containingNamespaceSymbol != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingNamespaceSymbol)} repeatedly");
                containingNamespaceSymbol = value;
            }
        }
        public new Name Name { get; }

        public ITypeSyntax? ReturnType { [DebuggerStepThrough] get; }
        public new FixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
        public IBodySyntax Body { [DebuggerStepThrough] get; }
        public new AcyclicPromise<FunctionSymbol> Symbol { get; }

        public FunctionDeclarationSyntax(
            NamespaceName containingNamespaceName,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            TextSpan nameSpan,
            Name name,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnType,
            IBodySyntax body)
            : base(span, file, accessModifier, nameSpan, name, parameters, new AcyclicPromise<FunctionSymbol>())
        {
            ContainingNamespaceName = containingNamespaceName;
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
            Body = body;
            Symbol = (AcyclicPromise<FunctionSymbol>)base.Symbol;
        }

        public override string ToString()
        {
            var returnType = ReturnType != null ? " -> " + ReturnType : "";
            return $"fn {Name}({string.Join(", ", Parameters)}){returnType} {Body}";
        }
    }
}
