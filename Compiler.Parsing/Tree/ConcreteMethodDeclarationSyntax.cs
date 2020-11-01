using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    // TODO No error is reported if IConcreteMethodDeclarationSyntax is missing
    internal class ConcreteMethodDeclarationSyntax : MethodDeclarationSyntax, IConcreteMethodDeclarationSyntax
    {
        public virtual IBodySyntax Body { get; }

        public ConcreteMethodDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            TextSpan nameSpan,
            Name name,
            ISelfParameterSyntax selfParameter,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnType,
            IBodySyntax body)
            : base(declaringClass, span, file, accessModifier, nameSpan, name, selfParameter,
                parameters, returnType)
        {
            Body = body;
        }

        public override string ToString()
        {
            var returnType = ReturnType != null ? " -> " + ReturnType : "";
            return $"fn {Name}({string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter))}){returnType} {Body}";
        }
    }
}
