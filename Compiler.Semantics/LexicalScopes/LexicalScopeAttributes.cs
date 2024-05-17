using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal static class LexicalScopeAttributes
{
    public static PackageNameScope Package_InheritedPackageNameScope_MainFacet(IPackageNode node)
        => new PackageNameScope(new[] { node.MainFacet },
            node.References.Append(node.IntrinsicsReference).Select(r => r.SymbolNode.MainFacet));

    public static PackageNameScope Package_InheritedPackageNameScope_TestingFacet(IPackageNode node)
        => new PackageNameScope(new[] { node.MainFacet, node.TestingFacet },
            node.References.Append(node.IntrinsicsReference).Select(r => r.SymbolNode.MainFacet)
                .Concat(node.References.Select(r => r.SymbolNode.TestingFacet)));

    public static LexicalScope CompilationUnit_LexicalScope(ICompilationUnitNode node)
        => BuildNamespaceScope(node.ContainingLexicalScope, node.ImplicitNamespaceName, node.UsingDirectives);

    private static NamespaceSearchScope BuildNamespaceScope(
        NamespaceSearchScope containingLexicalScope,
        NamespaceName namespaceName,
        IFixedList<IUsingDirectiveNode> usingDirectives)
    {
        var namespaceScope = GetNamespaceScope(containingLexicalScope, namespaceName);
        var lexicalScope = BuildUsingDirectivesScope(namespaceScope, usingDirectives);
        return lexicalScope;
    }

    private static NamespaceScope GetNamespaceScope(
        NamespaceSearchScope containingLexicalScope, NamespaceName namespaceName)
    {
        var lexicalScope = containingLexicalScope;
        foreach (var ns in namespaceName.Segments)
            lexicalScope = lexicalScope.CreateChildNamespaceScope(ns)!;
        // Either CreateChildNamespaceScope was called, or this is a compilation unit and the
        // original containingLexicalScope was a NamespaceScope.
        return (NamespaceScope)lexicalScope;
    }

    private static NamespaceSearchScope BuildUsingDirectivesScope(
        NamespaceScope containingScope,
        IFixedList<IUsingDirectiveNode> usingDirectives)
    {
        if (!usingDirectives.Any()) return containingScope;

        var globalScope = containingScope.PackageNames.UsingGlobalScope;
        // TODO put a NamespaceScope attribute on the using directive node for this
        // TODO report an error if the using directive refers to a namespace that doesn't exist
        var namespaceScopes = usingDirectives.Select(d => GetNamespaceScope(globalScope, d.Name));

        return new UsingDirectivesScope(containingScope, namespaceScopes);
    }

    public static NamespaceSearchScope NamespaceBlockDefinition_LexicalScope(INamespaceBlockDefinitionNode node)
        => BuildNamespaceScope(node.ContainingLexicalScope, node.DeclaredNames, node.UsingDirectives);

    public static LexicalScope TypeDeclaration_SupertypesLexicalScope(ITypeDefinitionNode node)
    {
        if (node.GenericParameters.Any())
            return new DeclarationScope(node.ContainingLexicalScope, node.GenericParameters);

        return node.ContainingLexicalScope;
    }

    public static LexicalScope TypeDeclaration_LexicalScope(ITypeDefinitionNode node)
        // TODO populate the scope with members
        => new DeclarationScope(node.SupertypesLexicalScope, Enumerable.Empty<INamedDeclarationNode>());

    public static LexicalScope TypeDeclaration_InheritedLexicalScope_Supertypes(ITypeDefinitionNode node)
        => node.SupertypesLexicalScope;

    public static LexicalScope TypeDeclaration_InheritedLexicalScope(ITypeDefinitionNode node)
        => node.LexicalScope;

    public static LexicalScope FunctionDeclaration_LexicalScope(IFunctionDefinitionNode node)
        => throw new System.NotImplementedException();
}
