using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal static class LexicalScopingAspect
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
            lexicalScope = lexicalScope.CreateChildNamespaceScope(ns)
                           ?? throw new UnreachableException("Should always be getting namespace names that correspond to definitions.");
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
    {
        var containingLexicalScope = node.ContainingLexicalScope;
        if (node.IsGlobalQualified)
            containingLexicalScope = containingLexicalScope.PackageNames.PackageGlobalScope;
        return BuildNamespaceScope(containingLexicalScope, node.DeclaredNames, node.UsingDirectives);
    }

    public static LexicalScope TypeDefinition_SupertypesLexicalScope(ITypeDefinitionNode node)
    {
        if (node.GenericParameters.Any())
            return new DeclarationScope(node.ContainingLexicalScope, node.GenericParameters);

        return node.ContainingLexicalScope;
    }

    public static LexicalScope TypeDefinition_LexicalScope(ITypeDefinitionNode node)
        // TODO populate the scope with members
        => new DeclarationScope(node.SupertypesLexicalScope, Enumerable.Empty<INamedDeclarationNode>());

    public static LexicalScope TypeDefinition_InheritedLexicalScope_Supertypes(ITypeDefinitionNode node)
        => node.SupertypesLexicalScope;

    public static LexicalScope TypeDefinition_InheritedLexicalScope(ITypeDefinitionNode node)
        => node.LexicalScope;

    public static LexicalScope FunctionDefinition_LexicalScope(IFunctionDefinitionNode node)
        // TODO create a type parameter scope when type parameters are supported
        => new DeclarationScope(node.ContainingLexicalScope, node.Parameters);

    public static LexicalScope BodyOrBlock_InheritedLexicalScope(IBodyOrBlockNode node, int statementIndex)
    {
        if (statementIndex == 0)
            return node.GetContainingLexicalScope();
        return node.Statements[statementIndex - 1].GetLexicalScope();
    }

    public static LexicalScope VariableDeclarationStatement_LexicalScope(IVariableDeclarationStatementNode node)
        => new DeclarationScope(node.ContainingLexicalScope, node);
}