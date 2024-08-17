using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface IGetterMethodDefinitionSyntax
{
    public static IGetterMethodDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IReturnSyntax @return,
        IBodySyntax body)
        // TODO allow AG equation to provide empty parameter list
        => Create(span, file, nameSpan, accessModifier, name, selfParameter,
            FixedList.Empty<INamedParameterSyntax>(), @return, body);
}
