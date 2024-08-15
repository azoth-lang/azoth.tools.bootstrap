using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class InvocableDefinitionSyntax : DefinitionSyntax, IInvocableDefinitionSyntax
{
    public IAccessModifierToken? AccessModifier { get; }
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; }

    protected InvocableDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName? name,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters)
        : base(span, file, name, nameSpan)
    {
        AccessModifier = accessModifier;
        Parameters = parameters.ToFixedList();
    }
}
