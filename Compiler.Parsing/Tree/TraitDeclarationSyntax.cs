using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class TraitDeclarationSyntax : TypeDeclarationSyntax<ITraitMemberDeclarationSyntax>, ITraitDeclarationSyntax
{
    public override FixedList<ITraitMemberDeclarationSyntax> Members { get; }

    public TraitDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        Name name,
        FixedList<IGenericParameterSyntax> genericParameters,
        FixedList<ITypeNameSyntax> superTypes,
        Func<ITraitDeclarationSyntax, (FixedList<ITraitMemberDeclarationSyntax>, TextSpan)> parseMembers)
        : base(containingNamespaceName, headerSpan, file, accessModifier, constModifier, moveModifier,
            nameSpan, name, genericParameters, superTypes)
    {
        var (members, bodySpan) = parseMembers(this);
        Members = members;
        Span = TextSpan.Covering(headerSpan, bodySpan);
    }

    public override string ToString()
    {
        var modifiers = "";
        var accessModifier = AccessModifier.ToAccessModifier();
        if (accessModifier != CST.AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (IsConst) modifiers += "const ";
        if (IsMove) modifiers += "move ";
        var generics = GenericParameters.Any() ? $"[{string.Join(", ", GenericParameters)}]" : "";
        return $"{modifiers}trait {Name}{generics} {{ … }}";
    }


}