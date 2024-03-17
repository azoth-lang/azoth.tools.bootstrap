using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class SetterMethodDeclarationSyntax : ConcreteMethodDeclarationSyntax, ISetterMethodDeclarationSyntax
{
    public override IReturnSyntax? Return => null;

    public SetterMethodDeclarationSyntax(
        ITypeDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        SimpleName name,
        IMethodSelfParameterSyntax selfParameter,
        IFixedList<INamedParameterSyntax> parameters,
        IBodySyntax body)
        : base(declaringType, span, file, accessModifier, nameSpan, name, selfParameter, parameters, body) { }

    public override string ToString()
        => $"set {Name}({string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter))}) {Body}";
}
