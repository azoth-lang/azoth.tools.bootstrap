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

internal class ClassDeclarationSyntax : TypeDeclarationSyntax<IClassMemberDeclarationSyntax>, IClassDeclarationSyntax
{
    public IAbstractKeywordToken? AbstractModifier { get; }
    public bool IsAbstract { get; }
    public ISupertypeNameSyntax? BaseTypeName { get; }
    public override IFixedList<IClassMemberDeclarationSyntax> Members { get; }
    public ConstructorSymbol? DefaultConstructorSymbol { get; private set; }

    public ClassDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IAbstractKeywordToken? abstractModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        string name,
        IFixedList<IGenericParameterSyntax> genericParameters,
        ISupertypeNameSyntax? baseTypeName,
        IFixedList<ISupertypeNameSyntax> supertypesNames,
        Func<IClassDeclarationSyntax, (IFixedList<IClassMemberDeclarationSyntax>, TextSpan)> parseMembers)
        : base(containingNamespaceName, headerSpan, file, accessModifier, constModifier, moveModifier,
            nameSpan, StandardName.Create(name, genericParameters.Count), genericParameters, supertypesNames)
    {
        AbstractModifier = abstractModifier;
        IsAbstract = AbstractModifier is not null;
        BaseTypeName = baseTypeName;
        var (members, bodySpan) = parseMembers(this);
        Members = members;
        Span = TextSpan.Covering(headerSpan, bodySpan);
    }

    public void CreateDefaultConstructor(ISymbolTreeBuilder symbolTree)
    {
        if (Members.Any(m => m is IConstructorDeclarationSyntax))
            return;

        if (DefaultConstructorSymbol is not null)
            throw new InvalidOperationException($"Can't {nameof(CreateDefaultConstructor)} twice");

        var constructorSymbol = ConstructorSymbol.CreateDefault(Symbol.Result);
        var selfParameterSymbol = new SelfParameterSymbol(constructorSymbol, false, constructorSymbol.SelfParameterType);

        symbolTree.Add(constructorSymbol);
        symbolTree.Add(selfParameterSymbol);
        DefaultConstructorSymbol = constructorSymbol;
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
        return $"{modifiers}class {Name}{generics} {{ … }}";
    }
}
