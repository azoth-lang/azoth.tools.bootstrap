using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AssociatedFunctionDefinitionSyntax : InvocableDefinitionSyntax, IAssociatedFunctionDefinitionSyntax
{
    public new IdentifierName Name { get; }
    public new IFixedList<INamedParameterSyntax> Parameters { get; }
    public override IFixedList<IParameterSyntax> AllParameters => Parameters;
    public IReturnSyntax? Return { get; }
    public IBodySyntax Body { get; }

    public AssociatedFunctionDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IFixedList<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, parameters)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
        Return = @return;
    }

    public override string ToString()
    {
        var returnType = Return is not null ? Return.ToString() : "";
        return $"fn {Name}({string.Join(", ", Parameters)}){returnType} {Body}";
    }
}
