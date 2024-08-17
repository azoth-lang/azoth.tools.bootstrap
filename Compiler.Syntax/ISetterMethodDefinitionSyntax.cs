using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface ISetterMethodDefinitionSyntax
{
    public static ISetterMethodDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IFixedList<INamedParameterSyntax> parameters,
        IBodySyntax body)
        // TODO allow AG equation to provide empty return
        => Create(span, file, nameSpan, accessModifier, name, selfParameter, parameters, null, body);
}
