using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class StructDefinitionSyntax : TypeDefinitionSyntax<IStructMemberDefinitionSyntax>, IStructDefinitionSyntax
{
    public override IFixedList<IStructMemberDefinitionSyntax> Members { get; }

    public StructDefinitionSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IStructKindKeywordToken? structKind,
        TextSpan nameSpan,
        string name,
        IFixedList<IGenericParameterSyntax> genericParameters,
        IFixedList<IStandardTypeNameSyntax> supertypesNames,
        IFixedList<IStructMemberDefinitionSyntax> members)
        : base(containingNamespaceName, span, file, accessModifier, constModifier, structKind as IMoveKeywordToken,
            nameSpan, StandardName.Create(name, genericParameters.Count), genericParameters, supertypesNames)
    {
        Members = members;
    }

    public override string ToString()
    {
        var modifiers = "";
        var accessModifier = AccessModifier.ToAccessModifier();
        if (accessModifier != CST.AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (ConstModifier is not null) modifiers += "const ";
        if (MoveModifier is not null) modifiers += "move ";
        var generics = GenericParameters.Any() ? $"[{string.Join(", ", GenericParameters)}]" : "";
        return $"{modifiers}class {Name.ToBareString()}{generics} {{ … }}";
    }
}
