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
    public override IFixedList<ITraitMemberDeclarationSyntax> Members { get; }

    public TraitDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        string name,
        FixedList<IGenericParameterSyntax> genericParameters,
        IFixedList<ITypeNameSyntax> supertypes,
        Func<ITraitDeclarationSyntax, (FixedList<ITraitMemberDeclarationSyntax>, TextSpan)> parseMembers)
        : base(containingNamespaceName, headerSpan, file, accessModifier, constModifier, moveModifier,
            nameSpan, StandardTypeName.Create(name, genericParameters.Count), genericParameters, supertypes)
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
