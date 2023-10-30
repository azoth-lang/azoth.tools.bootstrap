using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;

/// <summary>
/// Inherit symbols (e.g. methods and fields) from parent types.
/// </summary>
internal class TypeSymbolInheritor
{
    private readonly ISymbolTreeBuilder symbolTree;
    private readonly IDictionary<ITypeDeclarationSyntax, bool> processed;
    private readonly FixedDictionary<TypeSymbol, ITypeDeclarationSyntax> typeDeclarationsInPackage;

    public TypeSymbolInheritor(
        ISymbolTreeBuilder symbolTree,
        IEnumerable<ITypeDeclarationSyntax> typeDeclarations)
    {
        this.symbolTree = symbolTree;
        typeDeclarationsInPackage = typeDeclarations.ToFixedDictionary(t => (TypeSymbol)t.Symbol.Result);
        processed = typeDeclarationsInPackage.Values.ToDictionary(t => t, _ => false);
    }

    public void AddInheritedSymbols()
    {
        foreach (var typeDeclaration in processed.Keys)
            AddInheritedSymbols(typeDeclaration);
    }

    private void AddInheritedSymbols(ITypeDeclarationSyntax typeDeclaration)
    {
        if (processed[typeDeclaration]) return;

        var typeSymbol = typeDeclaration.Symbol.Result;

        if (typeDeclaration is IClassDeclarationSyntax { BaseTypeName: not null and var baseTypeName })
            AddInheritedSymbols(typeSymbol, baseTypeName);

        foreach (var superTypeName in typeDeclaration.SupertypeNames)
            AddInheritedSymbols(typeSymbol, superTypeName);

        processed[typeDeclaration] = true;
    }

    private void AddInheritedSymbols(ObjectTypeSymbol typeSymbol, ITypeNameSyntax supertypeName)
    {
        var supertypeSymbol = supertypeName.ReferencedSymbol.Result;
        if (supertypeSymbol is null)
            return;
        // Make sure super types in the same package have already had their symbols inherited
        if (typeDeclarationsInPackage.TryGetValue(supertypeSymbol, out var supertypeDeclaration))
            AddInheritedSymbols(supertypeDeclaration);

        AddInheritedSymbols(typeSymbol, supertypeSymbol);
    }

    private void AddInheritedSymbols(ObjectTypeSymbol typeSymbol, TypeSymbol baseSymbol)
    {
        var existingMembers = symbolTree.GetChildrenOf(typeSymbol).ToFixedSet();
        foreach (var symbol in symbolTree.GetChildrenOf(baseSymbol))
        {
            if (symbol is ConstructorSymbol)
                continue;

            // TODO don't inherit private symbols

            // TODO use proper override determination
            if (existingMembers.Any(s => s.Name == symbol.Name))
                continue;

            symbolTree.AddInherited(typeSymbol, symbol);
        }
    }
}
