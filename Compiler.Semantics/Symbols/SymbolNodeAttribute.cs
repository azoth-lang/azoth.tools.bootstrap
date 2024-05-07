using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolNodeAttribute
{
    public static IPackageSymbolNode Package(IPackageNode node)
        => new SemanticPackageSymbolNode(node, node.MainFacet.SymbolNode, node.TestingFacet.SymbolNode);

    public static IPackageFacetSymbolNode PackageFacet(IPackageFacetNode node)
    {
        var packageSymbol = node.PackageSymbol;
        var builder = new SemanticNamespaceSymbolNodeBuilder(packageSymbol);
        foreach (var cu in node.CompilationUnits)
            BuildNamespace(packageSymbol, cu.ImplicitNamespaceName, cu.Declarations);
        return new SemanticPackageFacetSymbolNode(builder.Build());

        void BuildMember(NamespaceSymbol namespaceSymbol, INamespaceMemberDeclarationNode declaration)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case INamespaceDeclarationNode n:
                    var containingNamespace = n.IsGlobalQualified ? packageSymbol : namespaceSymbol;
                    BuildNamespace(containingNamespace, n.DeclaredNames, n.Declarations);
                    break;
                case IPackageMemberDeclarationNode n:
                    builder.Add(namespaceSymbol, n.SymbolNode);
                    break;
            }
        }

        void BuildNamespace(NamespaceSymbol containingNamespace, NamespaceName name,
            IEnumerable<INamespaceMemberDeclarationNode> declarations)
        {
            var namespaceSymbol = builder.AddNamespace(containingNamespace, name);
            foreach (var declaration in declarations)
                BuildMember(namespaceSymbol, declaration);
        }
    }

    public static INamespaceSymbolNode CompilationUnit(ICompilationUnitNode node)
        => FindNamespace(node.ContainingSymbolNode.GlobalNamespace, node.ImplicitNamespaceName);

    private static INamespaceSymbolNode FindNamespace(INamespaceSymbolNode containingSymbolNode, NamespaceName ns)
    {
        var current = containingSymbolNode;
        foreach (var name in ns.Segments)
            current = current.MembersNamed(name).OfType<INamespaceSymbolNode>().Single();
        return current;
    }

    public static INamespaceSymbolNode CompilationUnitInherited(ICompilationUnitNode node)
        => node.ImplicitNamespaceSymbolNode;

    public static IPackageSymbolNode PackageReference(IPackageReferenceNode node)
        => new ReferencedPackageSymbolNode(node);

    public static FixedDictionary<IdentifierName, IPackageSymbolNode> PackageSymbolNodes(IPackageNode node)
        => node.References.Select(r => r.SymbolNode).Append(node.SymbolNode).ToFixedDictionary(n => n.AliasOrName ?? node.Symbol.Name);

    public static INamespaceSymbolNode NamespaceDeclarationContainingSymbolNode(INamespaceDeclarationNode node, INamespaceSymbolNode inheritedSymbolNode)
        => node.IsGlobalQualified ? inheritedSymbolNode.Facet.GlobalNamespace : inheritedSymbolNode;

    public static INamespaceSymbolNode NamespaceDeclaration(INamespaceDeclarationNode node)
        => FindNamespace(node.ContainingSymbolNode, node.DeclaredNames);

    public static INamespaceSymbolNode NamespaceDeclarationInherited(INamespaceDeclarationNode node)
        => node.SymbolNode;

    public static ITypeSymbolNode TypeDeclarationInherited(ITypeDeclarationNode node)
        => node.SymbolNode;

    public static IClassSymbolNode ClassDeclaration(IClassDeclarationNode node)
        => new SemanticClassSymbolNode(node);

    public static IStructSymbolNode StructDeclaration(IStructDeclarationNode node)
        => new SemanticStructSymbolNode(node);

    public static ITraitSymbolNode TraitDeclaration(ITraitDeclarationNode node)
        => new SemanticTraitSymbolNode(node);

    public static IFunctionSymbolNode FunctionDeclaration(IFunctionDeclarationNode node)
        => new SemanticFunctionSymbolNode(node);

    public static ITypeSymbolNode? StandardTypeName(IStandardTypeNameNode node)
        => LookupSymbolNodes(node).TrySingle();

    public static void StandardTypeNameContributeDiagnostics(IStandardTypeNameNode node, Diagnostics diagnostics)
    {
        if (node.ReferencedSymbolNode is not null)
            return;
        var symbolNodes = LookupSymbolNodes(node);
        switch (symbolNodes.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.Syntax.Span));
                break;
            case 1:
                // If there is only one match, then ReferencedSymbol is not null
                throw new UnreachableException();
            default:
                diagnostics.Add(NameBindingError.AmbiguousName(node.File, node.Syntax.Span));
                break;
        }
    }

    private static IFixedSet<ITypeSymbolNode> LookupSymbolNodes(IStandardTypeNameNode node)
        => node.ContainingLexicalScope.Lookup(node.Name).OfType<ITypeSymbolNode>().ToFixedSet();

    #region Construct for Symbols
    public static IChildSymbolNode Symbol(Symbol symbol)
        => symbol switch
        {
            NamespaceSymbol sym => new ReferencedNamespaceSymbolNode(sym),
            UserTypeSymbol sym => UserTypeSymbol(sym),
            FunctionSymbol sym => new ReferencedFunctionSymbolNode(sym),
            _ => throw new NotImplementedException(),
        };

    private static ReferencedTypeNode UserTypeSymbol(UserTypeSymbol symbol)
        => symbol.DeclaresType switch
        {
            StructType _ => new ReferencedStructSymbolNode(symbol),
            ObjectType t => t.IsClass switch
            {
                true => new ReferencedClassSymbolNode(symbol),
                false => new ReferencedTraitSymbolNode(symbol),
            },
            _ => throw ExhaustiveMatch.Failed(symbol.DeclaresType),
        };
    #endregion
}
