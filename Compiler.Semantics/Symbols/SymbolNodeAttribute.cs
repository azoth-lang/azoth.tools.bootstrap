using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolNodeAttribute
{
    public static IPackageSymbolNode Package(IPackageNode node)
        => new SemanticPackageSymbolNode(node,
            BuildForCompilationUnits(node.Symbol, node.CompilationUnits),
            BuildForCompilationUnits(node.Symbol, node.TestingCompilationUnits));

    private static IPackageFacetSymbolNode BuildForCompilationUnits(
        PackageSymbol packageSymbol,
        IEnumerable<ICompilationUnitNode> nodes)
    {
        var builder = new SemanticNamespaceSymbolNodeBuilder(packageSymbol);
        foreach (var node in nodes)
            BuildNamespace(packageSymbol, node.ImplicitNamespaceName, node.Declarations);
        return new SemanticPackageFacetSymbolNode(builder.Build());

        void Build(NamespaceSymbol namespaceSymbol, INamespaceMemberDeclarationNode declaration)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case INamespaceDeclarationNode n:
                    var containingNamespace = n.IsGlobalQualified ? packageSymbol : namespaceSymbol;
                    BuildNamespace(containingNamespace, n.DeclaredNames, n.Declarations);
                    break;
                case ITypeDeclarationNode n:
                    builder.Add(namespaceSymbol, BuildForType(n));
                    break;
                case IFunctionDeclarationNode n:
                    builder.Add(namespaceSymbol, BuildForFunction(n));
                    break;
            }
        }

        void BuildNamespace(NamespaceSymbol containingNamespace, NamespaceName name,
            IEnumerable<INamespaceMemberDeclarationNode> declarations)
        {
            var namespaceSymbol = builder.AddNamespace(containingNamespace, name);
            foreach (var declaration in declarations)
                Build(namespaceSymbol, declaration);
        }
    }

    private static ITypeSymbolNode BuildForType(ITypeDeclarationNode node)
        => node switch
        {
            IClassDeclarationNode n => new SemanticClassSymbolNode(n),
            IStructDeclarationNode n => new SemanticStructSymbolNode(n),
            ITraitDeclarationNode n => new SemanticTraitSymbolNode(n),
            _ => throw new NotImplementedException(),
        };

    private static IFunctionSymbolNode BuildForFunction(IFunctionDeclarationNode node)
        => new SemanticFunctionSymbolNode(node);

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

    public static ITypeSymbolNode TypeDeclaration(ITypeDeclarationNode node)
        => throw new NotImplementedException();

    public static ITypeSymbolNode TypeDeclarationInherited(ITypeDeclarationNode node)
        => node.SymbolNode;

    public static IChildSymbolNode Symbol(Symbol symbol)
        => symbol switch
        {
            NamespaceSymbol ns => new ReferencedNamespaceSymbolNode(ns),
            _ => throw new NotImplementedException(),
        };
}
