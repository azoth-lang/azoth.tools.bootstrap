using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class TraitDefinitionSyntax : TypeDefinitionSyntax<ITraitMemberDefinitionSyntax>, ITraitDefinitionSyntax
{
    public override IFixedList<ITraitMemberDefinitionSyntax> Members { get; }

    public TraitDefinitionSyntax(
        NamespaceName containingNamespaceName,
        ITypeDefinitionSyntax? declaringType,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        string name,
        IFixedList<IGenericParameterSyntax> genericParameters,
        IFixedList<IStandardTypeNameSyntax> supertypes,
        Func<ITraitDefinitionSyntax, (IFixedList<ITraitMemberDefinitionSyntax>, TextSpan)> parseMembers)
        : base(containingNamespaceName, declaringType, headerSpan, file, accessModifier, constModifier, moveModifier,
            nameSpan, StandardName.Create(name, genericParameters.Count), genericParameters, supertypes)
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
        return $"{modifiers}trait {Name.ToBareString()}{generics} {{ … }}";
    }
}