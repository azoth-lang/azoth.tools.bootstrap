using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class AssociatedFunctionDeclarationSyntax : InvocableDeclarationSyntax, IAssociatedFunctionDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        public new Name Name { get; }
        public new FixedList<INamedParameterSyntax> Parameters { get; }
        public ITypeSyntax? ReturnType { get; }
        public IBodySyntax Body { get; }
        public new AcyclicPromise<FunctionSymbol> Symbol { get; }

        public AssociatedFunctionDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            TextSpan nameSpan,
            Name name,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnTypeSyntax,
            IBodySyntax body)
            : base(span, file, accessModifier, nameSpan, name, parameters, new AcyclicPromise<FunctionSymbol>())
        {
            DeclaringClass = declaringClass;
            Name = name;
            Parameters = parameters;
            ReturnType = returnTypeSyntax;
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
