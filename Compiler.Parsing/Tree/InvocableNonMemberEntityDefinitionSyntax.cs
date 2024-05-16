using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class InvocableNonMemberEntityDefinitionSyntax : InvocableDefinitionSyntax, INonMemberEntityDefinitionSyntax
{
    public NamespaceName ContainingNamespaceName { get; }

    private NamespaceSymbol? containingNamespaceSymbol;
    public NamespaceSymbol ContainingNamespaceSymbol
    {
        get => containingNamespaceSymbol
               ?? throw new InvalidOperationException($"{ContainingNamespaceSymbol} not yet assigned");
        set
        {
            if (containingNamespaceSymbol is not null)
                throw new InvalidOperationException($"Can't set {nameof(ContainingNamespaceSymbol)} repeatedly");
            containingNamespaceSymbol = value;
        }
    }

    public new TypeName Name { get; }

    protected InvocableNonMemberEntityDefinitionSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        TextSpan nameSpan,
        IdentifierName name,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters,
        IPromise<InvocableSymbol> symbol)
        : base(span, file, accessModifier, nameSpan, name, parameters, symbol)
    {
        ContainingNamespaceName = containingNamespaceName;
        Name = name;
    }
}
