using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class InvocableNonMemberEntityDefinitionSyntax : InvocableDefinitionSyntax, INonMemberEntityDefinitionSyntax
{
    public NamespaceName ContainingNamespaceName { get; }

    public new TypeName Name { get; }

    protected InvocableNonMemberEntityDefinitionSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters)
        : base(span, file, accessModifier, nameSpan, name, parameters)
    {
        ContainingNamespaceName = containingNamespaceName;
        Name = name;
    }
}
