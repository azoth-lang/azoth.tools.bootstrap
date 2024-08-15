using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class StructDefinitionSyntax : TypeDefinitionSyntax<IStructMemberDefinitionSyntax>, IStructDefinitionSyntax
{
    public override IFixedList<IStructMemberDefinitionSyntax> Members { get; }
    public InitializerSymbol? DefaultInitializerSymbol { get; private set; }

    public StructDefinitionSyntax(
        NamespaceName containingNamespaceName,
        ITypeDefinitionSyntax? declaringType,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IStructKindKeywordToken? structKind,
        TextSpan nameSpan,
        string name,
        IFixedList<IGenericParameterSyntax> genericParameters,
        IFixedList<IStandardTypeNameSyntax> supertypesNames,
        Func<IStructDefinitionSyntax, (IFixedList<IStructMemberDefinitionSyntax>, TextSpan)> parseMembers)
        : base(containingNamespaceName, declaringType, headerSpan, file, accessModifier, constModifier, structKind as IMoveKeywordToken,
            nameSpan, StandardName.Create(name, genericParameters.Count), genericParameters, supertypesNames)
    {
        var (members, bodySpan) = parseMembers(this);
        Members = members;
        Span = TextSpan.Covering(headerSpan, bodySpan);
    }

    public void CreateDefaultInitializer(ISymbolTreeBuilder symbolTree)
    {
        if (Members.Any(m => m is IInitializerDefinitionSyntax))
            return;

        if (DefaultInitializerSymbol is not null)
            throw new InvalidOperationException($"Can't {nameof(CreateDefaultInitializer)} twice");

        var constructorSymbol = InitializerSymbol.CreateDefault(Symbol.Result);

        symbolTree.Add(constructorSymbol);
        DefaultInitializerSymbol = constructorSymbol;
    }

    public override string ToString()
    {
        var modifiers = "";
        var accessModifier = AccessModifier.ToAccessModifier();
        if (accessModifier != CST.AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (IsConst) modifiers += "const ";
        if (IsMove) modifiers += "move ";
        var generics = GenericParameters.Any() ? $"[{string.Join(", ", GenericParameters)}]" : "";
        return $"{modifiers}class {Name.ToBareString()}{generics} {{ … }}";
    }
}
