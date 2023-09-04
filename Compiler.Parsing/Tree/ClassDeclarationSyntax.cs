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
    public IClassCapabilityToken? CapabilityModifier { get; }
    public new Name Name { get; }
    public new AcyclicPromise<ObjectTypeSymbol> Symbol { get; }
    public FixedList<IMemberDeclarationSyntax> Members { get; }
    public ConstructorSymbol? DefaultConstructorSymbol { get; private set; }

    public ClassDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan headerSpan,
        CodeFile file,
        IAccessModifierToken? accessModifier,
        IClassCapabilityToken? capabilityModifier,
        TextSpan nameSpan,
        Name name,
        Func<IClassDeclarationSyntax, (FixedList<IMemberDeclarationSyntax>, TextSpan)> parseMembers)
        : base(headerSpan, file, name, nameSpan, new AcyclicPromise<ObjectTypeSymbol>())
    {
        ContainingNamespaceName = containingNamespaceName;
        AccessModifier = accessModifier;
        CapabilityModifier = capabilityModifier;
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

        var constructedType = Symbol.Result.DeclaresDataType;
        var constructorSymbol = new ConstructorSymbol(Symbol.Result, null, FixedList<DataType>.Empty);
        var selfParameterSymbol = new SelfParameterSymbol(constructorSymbol, constructedType.ToConstructorSelf());

        symbolTree.Add(constructorSymbol);
        symbolTree.Add(selfParameterSymbol);
        DefaultConstructorSymbol = constructorSymbol;
    }

    public override string ToString()
    {
        var capability = CapabilityModifier is null ? "" : CapabilityModifier + " ";
        return $"{capability}class {Name} {{ … }}";
    }
}
