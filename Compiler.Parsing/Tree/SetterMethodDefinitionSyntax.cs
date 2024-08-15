using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class SetterMethodDefinitionSyntax : ConcreteMethodDefinitionSyntax, ISetterMethodDefinitionSyntax
{
    public override MethodKind Kind => MethodKind.Setter;
    public override IReturnSyntax? Return => null;

    public SetterMethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        INamedParameterSyntax? parameter,
        IBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, selfParameter,
            parameter is not null ? FixedList.Create(parameter) : FixedList.Empty<INamedParameterSyntax>(), body)
    { }

    public override string ToString()
        => $"set {Name}({string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter))}) {Body}";
}
