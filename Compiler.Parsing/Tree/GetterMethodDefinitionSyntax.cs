using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class GetterMethodDefinitionSyntax : ConcreteMethodDefinitionSyntax, IGetterMethodDefinitionSyntax
{
    public override MethodKind Kind => MethodKind.Getter;
    public override IReturnSyntax Return { get; }

    public GetterMethodDefinitionSyntax(
        ITypeDefinitionSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IReturnSyntax @return,
        IBodySyntax body)
        : base(declaringType, span, file, accessModifier, nameSpan, name, selfParameter,
            FixedList.Empty<INamedParameterSyntax>(), body)
    {
        Return = @return;
    }

    public override string ToString()
        => $"get {Name}({SelfParameter}){Return} {Body}";
}
