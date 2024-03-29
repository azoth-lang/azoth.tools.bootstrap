using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class ConcreteMethodDeclarationSyntax : MethodDeclarationSyntax, IConcreteMethodDeclarationSyntax
{
    public virtual IBodySyntax Body { get; }

    protected ConcreteMethodDeclarationSyntax(
        ITypeDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IFixedList<INamedParameterSyntax> parameters,
        IBodySyntax body)
        : base(declaringType, span, file, accessModifier, nameSpan, name, selfParameter,
            parameters)
    {
        Body = body;
    }
}
