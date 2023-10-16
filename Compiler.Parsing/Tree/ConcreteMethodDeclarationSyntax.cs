using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

// TODO No error is reported if IConcreteMethodDeclarationSyntax is missing
internal class ConcreteMethodDeclarationSyntax : MethodDeclarationSyntax, IConcreteMethodDeclarationSyntax
{
    public virtual IBodySyntax Body { get; }

    public ConcreteMethodDeclarationSyntax(
        ITypeDeclarationSyntax declaringType,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        SimpleName name,
        ISelfParameterSyntax selfParameter,
        FixedList<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
        : base(declaringType, span, file, accessModifier, nameSpan, name, selfParameter,
            parameters, @return)
    {
        Body = body;
    }

    public override string ToString()
    {
        var @return = Return is not null ? Return.ToString() : "";
        return $"fn {Name}({string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter))}){@return} {Body}";
    }
}
