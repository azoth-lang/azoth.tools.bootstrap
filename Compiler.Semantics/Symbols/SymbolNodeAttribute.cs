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
    public static IPackageSymbolNode PackageSymbolNode(IPackageNode node)
        => new SemanticPackageSymbolNode(node,
            CompilationUnitsSymbolNode(node.Symbol, node.CompilationUnits),
            CompilationUnitsSymbolNode(node.Symbol, node.TestingCompilationUnits));

    private static INamespaceSymbolNode CompilationUnitsSymbolNode(
        PackageSymbol packageSymbol,
        IEnumerable<ICompilationUnitNode> nodes)
    {
        var builder = new SemanticNamespaceSymbolNodeBuilder(packageSymbol);
        foreach (var node in nodes)
            BuildNamespace(packageSymbol, node.ImplicitNamespaceName, node.Declarations);
        return builder.Build();

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
                    builder.Add(namespaceSymbol, TypeSymbolNode(n));
                    break;
                case IFunctionDeclarationNode n:
                    builder.Add(namespaceSymbol, FunctionSymbolNode(n));
                    break;
            }
        }

        void BuildNamespace(NamespaceSymbol containingNamespace, NamespaceName name,
            IEnumerable<INamespaceMemberDeclarationNode> declarations)
        {
            var namespaceSymbol = builder.AddNamespace(containingNamespace, name);

            foreach (var declaration in declarations) Build(namespaceSymbol, declaration);
        }
    }

    public static IPackageSymbolNode PackageReferenceSymbolNode(IPackageReferenceNode node)
        => new ReferencedPackageSymbolNode(node);

    public static FixedDictionary<IdentifierName, IPackageSymbolNode> PackageSymbolNodes(IPackageNode node)
        => node.References.Select(r => r.SymbolNode).Append(node.SymbolNode).ToFixedDictionary(n => n.AliasOrName);

    private static ITypeSymbolNode TypeSymbolNode(ITypeDeclarationNode node)
        => node switch
        {
            IClassDeclarationNode n => new SemanticClassSymbolNode(n),
            IStructDeclarationNode n => new SemanticStructSymbolNode(n),
            ITraitDeclarationNode n => new SemanticTraitSymbolNode(n),
            _ => throw new NotImplementedException(),
        };

    private static IFunctionSymbolNode FunctionSymbolNode(IFunctionDeclarationNode node)
        => new SemanticFunctionSymbolNode(node);

    public static IChildSymbolNode SymbolNode(Symbol symbol)
        => symbol switch
        {
            NamespaceSymbol ns => new ReferencedNamespaceSymbolNode(ns),
            _ => throw new NotImplementedException(),
        };
}
