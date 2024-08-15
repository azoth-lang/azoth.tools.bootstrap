using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class ConstructorDefinitionSyntax : InvocableDefinitionSyntax, IConstructorDefinitionSyntax
{
    public new IdentifierName? Name { get; }
    public IConstructorSelfParameterSyntax SelfParameter { get; }
    public new IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; }
    public IBlockBodySyntax Body { get; }

    public ConstructorDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName? name,
        IConstructorSelfParameterSyntax selfParameter,
        IFixedList<IConstructorOrInitializerParameterSyntax> parameters,
        IBlockBodySyntax body)
        : base(span, file, accessModifier, nameSpan, name, parameters)
    {
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Body = body;
    }

    public override string ToString()
    {
        var parameters = string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter));
        return Name is null ? $"new({parameters})" : $"new {Name}({parameters})";
    }
}
