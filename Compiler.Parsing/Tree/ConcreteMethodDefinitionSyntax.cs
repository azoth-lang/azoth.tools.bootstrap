using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class ConcreteMethodDefinitionSyntax : MethodDefinitionSyntax, IConcreteMethodDefinitionSyntax
{
    public virtual IBodySyntax Body { get; }

    protected ConcreteMethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IFixedList<INamedParameterSyntax> parameters,
        IBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, selfParameter,
            parameters)
    {
        Body = body;
    }
}
