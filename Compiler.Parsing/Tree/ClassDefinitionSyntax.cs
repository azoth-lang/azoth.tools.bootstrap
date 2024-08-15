using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ClassDefinitionSyntax : TypeDefinitionSyntax<IClassMemberDefinitionSyntax>, IClassDefinitionSyntax
{
    public IAbstractKeywordToken? AbstractModifier { get; }
    public bool IsAbstract { get; }
    public IStandardTypeNameSyntax? BaseTypeName { get; }
    public override IFixedList<IClassMemberDefinitionSyntax> Members { get; }

    public ClassDefinitionSyntax(
        NamespaceName containingNamespaceName,
        ITypeDefinitionSyntax? declaringType,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IAbstractKeywordToken? abstractModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        string name,
        IFixedList<IGenericParameterSyntax> genericParameters,
        IStandardTypeNameSyntax? baseTypeName,
        IFixedList<IStandardTypeNameSyntax> supertypesNames,
        Func<IClassDefinitionSyntax, (IFixedList<IClassMemberDefinitionSyntax>, TextSpan)> parseMembers)
        : base(containingNamespaceName, declaringType, headerSpan, file, accessModifier, constModifier, moveModifier,
            nameSpan, StandardName.Create(name, genericParameters.Count), genericParameters, supertypesNames)
    {
        AbstractModifier = abstractModifier;
        IsAbstract = AbstractModifier is not null;
        BaseTypeName = baseTypeName;
        var (members, bodySpan) = parseMembers(this);
        Members = members;
        Span = TextSpan.Covering(headerSpan, bodySpan);
    }

    public override string ToString()
    {
        var modifiers = "";
        var accessModifier = AccessModifier.ToAccessModifier();
        if (accessModifier != CST.AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (IsAbstract) modifiers += "abstract ";
        if (IsConst) modifiers += "const ";
        if (IsMove) modifiers += "move ";
        var generics = GenericParameters.Any() ? $"[{string.Join(", ", GenericParameters)}]" : "";
        return $"{modifiers}class {Name.ToBareString()}{generics} {{ … }}";
    }
}
