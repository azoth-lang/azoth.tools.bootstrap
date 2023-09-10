using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ClassDeclarationSyntax : DeclarationSyntax, IClassDeclarationSyntax
{
    public NamespaceName ContainingNamespaceName { get; }

    private NamespaceOrPackageSymbol? containingNamespaceSymbol;
    public NamespaceOrPackageSymbol ContainingNamespaceSymbol
    {
        get => containingNamespaceSymbol
               ?? throw new InvalidOperationException($"{ContainingNamespaceSymbol} not yet assigned");
        set
        {
            if (containingNamespaceSymbol != null)
                throw new InvalidOperationException($"Can't set {nameof(ContainingNamespaceSymbol)} repeatedly");
            containingNamespaceSymbol = value;
        }
    }

    public IAccessModifierToken? AccessModifier { get; }
    public IConstKeywordToken? ConstModifier { get; }
    public bool IsConst { get; }
    public IMoveKeywordToken? MoveModifier { get; }
    public bool IsMove { get; }
    public new Name Name { get; }
    public new AcyclicPromise<ObjectTypeSymbol> Symbol { get; }
    public FixedList<IMemberDeclarationSyntax> Members { get; }
    public ConstructorSymbol? DefaultConstructorSymbol { get; private set; }

    public ClassDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        TextSpan nameSpan,
        Name name,
        Func<IClassDeclarationSyntax, (FixedList<IMemberDeclarationSyntax>, TextSpan)> parseMembers)
        : base(headerSpan, file, name, nameSpan, new AcyclicPromise<ObjectTypeSymbol>())
    {
        ContainingNamespaceName = containingNamespaceName;
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        IsConst = ConstModifier is not null;
        MoveModifier = moveModifier;
        IsMove = MoveModifier is not null;
        Name = name;
        var (members, bodySpan) = parseMembers(this);
        Members = members;
        Span = TextSpan.Covering(headerSpan, bodySpan);
        Symbol = (AcyclicPromise<ObjectTypeSymbol>)base.Symbol;
    }

    public void CreateDefaultConstructor(SymbolTreeBuilder symbolTree)
    {
        if (Members.Any(m => m is IConstructorDeclarationSyntax))
            return;

        if (DefaultConstructorSymbol is not null)
            throw new InvalidOperationException($"Can't {nameof(CreateDefaultConstructor)} twice");

        var constructedType = Symbol.Result.DeclaresType;
        var constructorSymbol = new ConstructorSymbol(Symbol.Result, null, FixedList<DataType>.Empty);
        var selfParameterSymbol = new SelfParameterSymbol(constructorSymbol, constructedType.ToConstructorSelf());

        symbolTree.Add(constructorSymbol);
        symbolTree.Add(selfParameterSymbol);
        DefaultConstructorSymbol = constructorSymbol;
    }

    public override string ToString()
    {
        var modifiers = "";
        var accessModifier = AccessModifier.ToAccessModifier();
        if (accessModifier != CST.AccessModifier.Private) modifiers += accessModifier.ToSourceString() + " ";
        if (IsConst) modifiers += "const ";
        if (IsMove) modifiers += "move ";
        return $"{modifiers}class {Name} {{ … }}";
    }
}
