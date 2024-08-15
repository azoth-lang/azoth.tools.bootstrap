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
    public IStandardTypeNameSyntax? BaseTypeName { get; }
    public override IFixedList<IClassMemberDefinitionSyntax> Members { get; }

    public ClassDefinitionSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
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
        IFixedList<IClassMemberDefinitionSyntax> members)
        : base(containingNamespaceName, span, file, accessModifier, constModifier, moveModifier,
            nameSpan, StandardName.Create(name, genericParameters.Count), genericParameters, supertypesNames)
    {
        AbstractModifier = abstractModifier;
        BaseTypeName = baseTypeName;
        Members = members;
    }

    public override string ToString()
    {
        var modifiers = "";
        var accessModifier = AccessModifier.ToAccessModifier();
        if (accessModifier != CST.AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (AbstractModifier is not null) modifiers += "abstract ";
        if (ConstModifier is not null) modifiers += "const ";
        if (MoveModifier is not null) modifiers += "move ";
        var generics = GenericParameters.Any() ? $"[{string.Join(", ", GenericParameters)}]" : "";
        return $"{modifiers}class {Name.ToBareString()}{generics} {{ … }}";
    }
}
