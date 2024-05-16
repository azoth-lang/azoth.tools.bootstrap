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
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolNodeAttributes
{
    public static IPackageDeclarationNode Package(IPackageNode node)
        => new SemanticPackageSymbolNode(node, node.MainFacet.SymbolNode, node.TestingFacet.SymbolNode);

    public static IPackageFacetDeclarationNode PackageFacet(IPackageFacetNode node)
    {
        var packageSymbol = node.PackageSymbol;
        var builder = new SemanticNamespaceSymbolNodeBuilder(packageSymbol);
        foreach (var cu in node.CompilationUnits)
            BuildNamespace(packageSymbol, cu.ImplicitNamespaceName, cu.Definitions);
        return new SemanticPackageFacetSymbolNode(builder.Build());

        void BuildMember(NamespaceSymbol namespaceSymbol, INamespaceMemberDefinitionNode declaration)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case INamespaceDefinitionNode n:
                    var containingNamespace = n.IsGlobalQualified ? packageSymbol : namespaceSymbol;
                    BuildNamespace(containingNamespace, n.DeclaredNames, n.Definitions);
                    break;
                case IPackageMemberDefinitionNode n:
                    builder.Add(namespaceSymbol, n.SymbolNode);
                    break;
            }
        }

        void BuildNamespace(NamespaceSymbol containingNamespace, NamespaceName name,
            IEnumerable<INamespaceMemberDefinitionNode> declarations)
        {
            var namespaceSymbol = builder.AddNamespace(containingNamespace, name);
            foreach (var declaration in declarations)
                BuildMember(namespaceSymbol, declaration);
        }
    }

    public static INamespaceDeclarationNode CompilationUnit(ICompilationUnitNode node)
        => FindNamespace(node.ContainingDeclarationNode.GlobalNamespace, node.ImplicitNamespaceName);

    private static INamespaceDeclarationNode FindNamespace(INamespaceDeclarationNode containingDeclarationNode, NamespaceName ns)
    {
        var current = containingDeclarationNode;
        foreach (var name in ns.Segments)
            current = current.MembersNamed(name).OfType<INamespaceDeclarationNode>().Single();
        return current;
    }

    public static INamespaceDeclarationNode CompilationUnit_InheritedContainingSymbolNode(ICompilationUnitNode node)
        => node.ImplicitNamespaceSymbolNode;

    public static IPackageDeclarationNode PackageReference(IPackageReferenceNode node)
        => new PackageSymbolNode(node);

    public static FixedDictionary<IdentifierName, IPackageDeclarationNode> Package_SymbolNodes(IPackageNode node)
        => node.References.Select(r => r.SymbolNode).Append(node.SymbolNode).ToFixedDictionary(n => n.AliasOrName ?? node.Symbol.Name);

    public static INamespaceDeclarationNode NamespaceDeclaration_ContainingSymbolNode(INamespaceDefinitionNode node, INamespaceDeclarationNode inheritedDeclarationNode)
        => node.IsGlobalQualified ? inheritedDeclarationNode.Facet.GlobalNamespace : inheritedDeclarationNode;

    public static INamespaceDeclarationNode NamespaceDeclaration_SymbolNode(INamespaceDefinitionNode node)
        => FindNamespace(node.ContainingDeclarationNode, node.DeclaredNames);

    public static INamespaceDeclarationNode NamespaceDeclaration_InheritedContainingSymbolNode(INamespaceDefinitionNode node)
        => node.SymbolNode;

    public static bool Attribute_InheritedIsAttributeType_Child(IAttributeNode _) => true;

    public static IUserTypeDeclarationNode TypeDeclaration_InheritedContainingSymbolNode(ITypeDefinitionNode node)
        => node.SymbolNode;

    public static bool TypeDeclaration_InheritedIsAttributeType(ITypeDefinitionNode _)
        => false;

    public static IClassDeclarationNode ClassDeclaration_SymbolNode(IClassDefinitionNode node)
        => new SemanticClassSymbolNode(node);

    /// <remarks>This needs to be lazy computed because the
    /// <see cref="IClassDefinitionNode.DefaultConstructorSymbol"/> is dependent on the class symbol.</remarks>
    public static IFixedList<IClassMemberDeclarationNode> ClassDeclaration_MembersSymbolNodes(IClassDefinitionNode node)
    {
        var memberSymbolNodes = ClassMembers(node.Members);

        var defaultConstructorSymbol = node.DefaultConstructorSymbol;
        if (defaultConstructorSymbol is not null)
            memberSymbolNodes = memberSymbolNodes.Append(ConstructorSymbol(defaultConstructorSymbol));

        return memberSymbolNodes.ToFixedList();
    }

    private static IEnumerable<IClassMemberDeclarationNode> ClassMembers(IEnumerable<IClassMemberDefinitionNode> nodes)
        => nodes.Select(n => n.SymbolNode);

    public static IStructDeclarationNode StructDeclaration(IStructDefinitionNode node)
        => new SemanticStructSymbolNode(node);

    public static ITraitDeclarationNode TraitDeclaration(ITraitDefinitionNode node)
        => new SemanticTraitSymbolNode(node);

    public static IGenericParameterDeclarationNode GenericParameter(GenericParameterNode node)
        => new SemanticGenericParameterDeclarationNode(node);

    public static IFunctionDeclarationNode FunctionDeclaration(IFunctionDefinitionNode node)
        => new SemanticFunctionSymbolNode(node);

    public static bool FunctionDeclaration_InheritedIsAttributeType(IFunctionDefinitionNode node)
        => false;

    public static ITypeDeclarationNode? StandardTypeName_ReferencedSymbolNode(IStandardTypeNameNode node)
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

    private static IFixedSet<ITypeDeclarationNode> LookupSymbolNodes(IStandardTypeNameNode node, bool withAttributeSuffix = false)
    {
        var name = withAttributeSuffix ? node.Name + SpecialNames.AttributeSuffix : node.Name;
        return node.ContainingLexicalScope.Lookup(name).OfType<ITypeDeclarationNode>().ToFixedSet();
    }

    public static IMethodDeclarationNode MethodDeclaration_SymbolNode(IMethodDefinitionNode node)
        => new SemanticMethodSymbolNode(node);

    public static IConstructorDeclarationNode ConstructorDeclaration_SymbolNode(IConstructorDefinitionNode node)
        => new SemanticConstructorSymbolNode(node);

    public static IFieldDeclarationNode FieldDeclaration_SymbolNode(IFieldDefinitionNode node)
        => new SemanticFieldSymbolNode(node);

    public static IFieldDeclarationNode? FieldParameter_ReferencedSymbolNode(IFieldParameterNode node)
        // TODO report error for field parameter without referenced field
        => node.ContainingTypeDefinition.Members.OfType<IFieldDefinitionNode>().FirstOrDefault(f => f.Name == node.Name)?.SymbolNode;

    public static IAssociatedFunctionDeclarationNode AssociatedFunction_SymbolNode(IAssociatedFunctionDefinitionNode node)
        => new SemanticAssociatedFunctionDeclarationNode(node);

    #region Construct for Symbols
    public static IFacetChildDeclarationNode Symbol(Symbol symbol)
        => symbol switch
        {
            NamespaceSymbol sym => new NamespaceSymbolNode(sym),
            TypeSymbol sym => TypeSymbol(sym),
            InvocableSymbol sym => InvocableSymbol(sym),
            BindingSymbol sym => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };

    private static IUserTypeDeclarationNode TypeSymbol(TypeSymbol symbol)
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

    private static IUserTypeDeclarationNode UserTypeSymbol(UserTypeSymbol symbol)
        => symbol.DeclaresType switch
        {
            StructType _ => new StructSymbolNode(symbol),
            ObjectType t => t.IsClass switch
            {
                true => new ClassSymbolNode(symbol),
                false => new TraitSymbolNode(symbol),
            },
            _ => throw ExhaustiveMatch.Failed(symbol.DeclaresType),
        };

    private static IFunctionDeclarationNode InvocableSymbol(InvocableSymbol symbol)
        => symbol switch
        {
            FunctionSymbol sym => FunctionSymbol(sym),
            _ => throw new NotImplementedException(),
        };

    private static IFunctionDeclarationNode FunctionSymbol(FunctionSymbol sym)
        => new FunctionSymbolNode(sym);

    private static IConstructorDeclarationNode ConstructorSymbol(ConstructorSymbol sym)
        => new ConstructorSymbolNode(sym);
    #endregion
}
