using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class AbstractMethodDefinitionSyntax : MethodDefinitionSyntax, IAbstractMethodDefinitionSyntax
{
    public override IReturnSyntax? Return { get; }

    public AbstractMethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IFixedList<INamedParameterSyntax> parameters,
        IReturnSyntax? @return)
        : base(span, file, accessModifier, nameSpan, name,
            selfParameter, parameters)
    {
        Return = @return;
    }

    public override string ToString()
    {
        var @return = Return is not null ? Return.ToString() : "";
        return $"fn {Name}({string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter))}){@return};";
    }
}
