using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class FunctionDefinitionSyntax : InvocableDefinitionSyntax, IFunctionDefinitionSyntax
{
    public NamespaceName ContainingNamespaceName { get; }
    public IFixedList<IAttributeSyntax> Attributes { get; }
    public new IdentifierName Name { get; }
    public new IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }

    public FunctionDefinitionSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
        CodeFile file,
        IFixedList<IAttributeSyntax> attributes,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IFixedList<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, parameters)
    {
        ContainingNamespaceName = containingNamespaceName;
        Name = name;
        Parameters = parameters;
        Body = body;
        Attributes = attributes;
        Return = @return;
    }

    public override string ToString()
    {
        var @return = Return is not null ? Return.ToString() : "";
        return $"fn {Name}({string.Join(", ", Parameters)}){@return} {Body}";
    }
}
