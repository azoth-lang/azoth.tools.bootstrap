using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class GetterMethodDefinitionSyntax : ConcreteMethodDefinitionSyntax, IGetterMethodDefinitionSyntax
{
    public override IReturnSyntax Return { get; }

    public GetterMethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IReturnSyntax @return,
        IBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, selfParameter,
            FixedList.Empty<INamedParameterSyntax>(), body)
    {
        Return = @return;
    }

    public override string ToString()
        => $"get {Name}({SelfParameter}){Return} {Body}";
}
