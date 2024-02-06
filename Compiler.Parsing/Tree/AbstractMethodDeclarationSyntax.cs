using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AbstractMethodDeclarationSyntax : MethodDeclarationSyntax, IAbstractMethodDeclarationSyntax
{
    public AbstractMethodDeclarationSyntax(
        ITypeDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        SimpleName name,
        IMethodSelfParameterSyntax selfParameter,
        FixedList<INamedParameterSyntax> parameters,
        IReturnSyntax? @return)
        : base(declaringType, span, file, accessModifier, nameSpan, name,
            selfParameter, parameters, @return)
    {
    }

    public override string ToString()
    {
        var @return = Return is not null ? Return.ToString() : "";
        return $"fn {Name}({string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter))}){@return};";
    }
}
