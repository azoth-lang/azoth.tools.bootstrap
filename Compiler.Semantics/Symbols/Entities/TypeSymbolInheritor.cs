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
    private readonly SymbolTreeBuilder symbolTree;
    private readonly IDictionary<ITypeDeclarationSyntax, bool> processed;
    private readonly FixedDictionary<TypeSymbol, ITypeDeclarationSyntax> typeDeclarations;

    public TypeSymbolInheritor(
        SymbolTreeBuilder symbolTree,
        IEnumerable<ITypeDeclarationSyntax> typeDeclarations)
    {
        this.symbolTree = symbolTree;
        this.typeDeclarations = typeDeclarations.ToFixedDictionary(t => (TypeSymbol)t.Symbol.Result);
        processed = this.typeDeclarations.Values.ToDictionary(t => t, _ => false);
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
        if (supertypeSymbol is not null && typeDeclarations.TryGetValue(supertypeSymbol, out var supertypeDeclaration))
        {
            AddInheritedSymbols(supertypeDeclaration);
            AddInheritedSymbols(typeSymbol, supertypeSymbol);
        }
    }

    private void AddInheritedSymbols(ObjectTypeSymbol typeSymbol, TypeSymbol baseSymbol)
    {
        var existingMembers = symbolTree.Children(typeSymbol).ToFixedSet();
        foreach (var symbol in symbolTree.Children(baseSymbol))
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
