using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolNodeAttributes
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

    public static INamespaceSymbolNode CompilationUnit_InheritedContainingSymbolNode(ICompilationUnitNode node)
        => node.ImplicitNamespaceSymbolNode;

    public static IPackageSymbolNode PackageReference(IPackageReferenceNode node)
        => new ReferencedPackageSymbolNode(node);

    public static FixedDictionary<IdentifierName, IPackageSymbolNode> Package_SymbolNodes(IPackageNode node)
        => node.References.Select(r => r.SymbolNode).Append(node.SymbolNode).ToFixedDictionary(n => n.AliasOrName ?? node.Symbol.Name);

    public static INamespaceSymbolNode NamespaceDeclaration_ContainingSymbolNode(INamespaceDeclarationNode node, INamespaceSymbolNode inheritedSymbolNode)
        => node.IsGlobalQualified ? inheritedSymbolNode.Facet.GlobalNamespace : inheritedSymbolNode;

    public static INamespaceSymbolNode NamespaceDeclaration_SymbolNode(INamespaceDeclarationNode node)
        => FindNamespace(node.ContainingSymbolNode, node.DeclaredNames);

    public static INamespaceSymbolNode NamespaceDeclaration_InheritedContainingSymbolNode(INamespaceDeclarationNode node)
        => node.SymbolNode;

    public static bool Attribute_InheritedIsAttributeType_Child(IAttributeNode _) => true;

    public static IUserTypeSymbolNode TypeDeclaration_InheritedContainingSymbolNode(ITypeDeclarationNode node)
        => node.SymbolNode;

    public static bool TypeDeclaration_InheritedIsAttributeType(ITypeDeclarationNode _)
        => false;

    public static IClassSymbolNode ClassDeclaration_SymbolNode(IClassDeclarationNode node)
        => new SemanticClassSymbolNode(node);

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IClassDeclarationNode.DefaultConstructorSymbol"/> is dependent on the class symbol.</remarks>
    public static IFixedList<IClassMemberSymbolNode> ClassDeclaration_MembersSymbolNodes(IClassDeclarationNode node)
    {
        var memberSymbolNodes = ClassMembers(node.Members);

        var defaultConstructorSymbol = node.DefaultConstructorSymbol;
        if (defaultConstructorSymbol is not null)
            memberSymbolNodes = memberSymbolNodes.Append(ConstructorSymbol(defaultConstructorSymbol));

        return memberSymbolNodes.ToFixedList();
    }

    private static IEnumerable<IClassMemberSymbolNode> ClassMembers(IEnumerable<IClassMemberDeclarationNode> nodes)
        => nodes.Select(n => n.SymbolNode);

    public static IStructSymbolNode StructDeclaration(IStructDeclarationNode node)
        => new SemanticStructSymbolNode(node);

    public static ITraitSymbolNode TraitDeclaration(ITraitDeclarationNode node)
        => new SemanticTraitSymbolNode(node);

    public static IGenericParameterSymbolNode GenericParameter(GenericParameterNode node)
        => new SemanticGenericParameterSymbolNode(node);

    public static IFunctionSymbolNode FunctionDeclaration(IFunctionDeclarationNode node)
        => new SemanticFunctionSymbolNode(node);

    public static bool FunctionDeclaration_InheritedIsAttributeType(IFunctionDeclarationNode node)
        => false;

    public static ITypeSymbolNode? StandardTypeName_ReferencedSymbolNode(IStandardTypeNameNode node)
    {
        var symbolNode = LookupSymbolNodes(node).TrySingle();
        if (node.IsAttributeType)
            symbolNode ??= LookupSymbolNodes(node, withAttributeSuffix: true).TrySingle();
        return symbolNode;
    }

    public static void StandardTypeName_ContributeDiagnostics(IStandardTypeNameNode node, Diagnostics diagnostics)
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

    private static IFixedSet<ITypeSymbolNode> LookupSymbolNodes(IStandardTypeNameNode node, bool withAttributeSuffix = false)
    {
        var name = withAttributeSuffix ? node.Name + SpecialNames.AttributeSuffix : node.Name;
        return node.ContainingLexicalScope.Lookup(name).OfType<ITypeSymbolNode>().ToFixedSet();
    }

    public static IMethodSymbolNode MethodDeclaration_SymbolNode(IMethodDeclarationNode node)
        => new SemanticMethodSymbolNode(node);

    public static IConstructorSymbolNode ConstructorDeclaration_SymbolNode(IConstructorDeclarationNode node)
        => new SemanticConstructorSymbolNode(node);

    public static IFieldSymbolNode FieldDeclaration_SymbolNode(IFieldDeclarationNode node)
        => new SemanticFieldSymbolNode(node);

    public static IFieldSymbolNode? FieldParameter_ReferencedSymbolNode(IFieldParameterNode node)
        // TODO report error for field parameter without referenced field
        => node.ContainingTypeDeclaration.Members.OfType<IFieldDeclarationNode>().FirstOrDefault(f => f.Name == node.Name)?.SymbolNode;

    public static IAssociatedFunctionSymbolNode AssociatedFunction_SymbolNode(IAssociatedFunctionDeclarationNode node)
        => new AssociatedFunctionSymbolNode(node);

    #region Construct for Symbols
    public static IDeclarationSymbolNode Symbol(Symbol symbol)
        => symbol switch
        {
            NamespaceSymbol sym => new ReferencedNamespaceSymbolNode(sym),
            TypeSymbol sym => TypeSymbol(sym),
            InvocableSymbol sym => InvocableSymbol(sym),
            BindingSymbol sym => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };

    private static IUserTypeSymbolNode TypeSymbol(TypeSymbol symbol)
        => symbol switch
        {
            UserTypeSymbol sym => UserTypeSymbol(sym),
            // These will be needed because the generic parameter type could be used in a type expression
            GenericParameterTypeSymbol sym => throw new NotImplementedException(),
            EmptyTypeSymbol _
                => throw new NotSupportedException("Symbol node for empty type not supported. Primitives not name bound through symbol nodes."),
            PrimitiveTypeSymbol _
                => throw new NotSupportedException("Symbol node for primitive type not supported. Primitives not name bound through symbol nodes."),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };

    private static IUserTypeSymbolNode UserTypeSymbol(UserTypeSymbol symbol)
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

    private static IFunctionSymbolNode InvocableSymbol(InvocableSymbol symbol)
        => symbol switch
        {
            FunctionSymbol sym => FunctionSymbol(sym),
            _ => throw new NotImplementedException(),
        };

    private static IFunctionSymbolNode FunctionSymbol(FunctionSymbol sym)
        => new ReferencedFunctionSymbolNode(sym);

    private static IConstructorSymbolNode ConstructorSymbol(ConstructorSymbol sym)
        => new ReferencedConstructorSymbolNode(sym);
    #endregion
}
