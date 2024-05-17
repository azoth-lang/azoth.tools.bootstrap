using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolNodeAttributes
{
    public static INamespaceDefinitionNode PackageFacet_GlobalNamespace(IPackageFacetNode node)
    {
        var packageSymbol = node.PackageSymbol;
        var builder = new SemanticNamespaceSymbolNodeBuilder(packageSymbol);
        foreach (var cu in node.CompilationUnits)
            BuildNamespace(packageSymbol, cu.ImplicitNamespaceName, cu.Definitions);
        return Child.Attach(node, builder.Build());

        void BuildMember(NamespaceSymbol namespaceSymbol, INamespaceBlockMemberDefinitionNode definition)
        {
            switch (definition)
            {
                default:
                    throw ExhaustiveMatch.Failed(definition);
                case INamespaceBlockDefinitionNode n:
                    var containingNamespace = n.IsGlobalQualified ? packageSymbol : namespaceSymbol;
                    BuildNamespace(containingNamespace, n.DeclaredNames, n.Members);
                    break;
                case IFunctionDefinitionNode n:
                    builder.Add(namespaceSymbol, n);
                    break;
                case ITypeDefinitionNode n:
                    builder.Add(namespaceSymbol, n);
                    break;
            }
        }

        void BuildNamespace(NamespaceSymbol containingNamespace, NamespaceName name,
            IEnumerable<INamespaceBlockMemberDefinitionNode> declarations)
        {
            var namespaceSymbol = builder.AddNamespace(containingNamespace, name);
            foreach (var declaration in declarations)
                BuildMember(namespaceSymbol, declaration);
        }
    }

    public static INamespaceDefinitionNode CompilationUnit_ImplicitNamespaceDeclaration(ICompilationUnitNode node)
        => FindNamespace(node.ContainingDeclaration.GlobalNamespace, node.ImplicitNamespaceName);

    private static INamespaceDefinitionNode FindNamespace(INamespaceDefinitionNode containingDeclarationNode, NamespaceName ns)
    {
        // TODO rework
        var current = containingDeclarationNode;
        foreach (var name in ns.Segments)
            current = current.MembersNamed(name).OfType<INamespaceDefinitionNode>().Single();
        return current;
    }

    public static INamespaceDeclarationNode CompilationUnit_InheritedContainingDeclaration(ICompilationUnitNode node)
        => node.ImplicitNamespace;

    public static IPackageSymbolNode PackageReference_SymbolNode(IPackageReferenceNode node)
        => new PackageSymbolNode(node);

    public static FixedDictionary<IdentifierName, IPackageDeclarationNode> Package_PackageDeclarations(IPackageNode node)
        => node.References.Select(r => r.SymbolNode).Append<IPackageDeclarationNode>(node)
               .ToFixedDictionary(n => n.AliasOrName ?? node.Symbol.Name);

    public static INamespaceDefinitionNode NamespaceBlockDefinition_ContainingDeclaration(INamespaceBlockDefinitionNode node)
        => node.IsGlobalQualified ? node.Facet.GlobalNamespace : node.ContainingDeclaration;

    public static INamespaceDefinitionNode NamespaceBlockDefinition_Declaration(INamespaceBlockDefinitionNode node)
        => FindNamespace(node.ContainingNamespace, node.DeclaredNames);

    public static INamespaceDeclarationNode NamespaceBlockDefinition_InheritedContainingDeclaration(INamespaceBlockDefinitionNode node)
        => node.Definition;

    public static bool Attribute_InheritedIsAttributeType_Child(IAttributeNode _) => true;

    public static IUserTypeDeclarationNode TypeDeclaration_InheritedContainingDeclaration(ITypeDefinitionNode node)
        => node;

    public static bool TypeDeclaration_InheritedIsAttributeType(ITypeDefinitionNode _)
        => false;

    public static bool FunctionDeclaration_InheritedIsAttributeType(IFunctionDefinitionNode node)
        => false;

    public static ITypeDeclarationNode? StandardTypeName_ReferencedDeclaration(IStandardTypeNameNode node)
    {
        var symbolNode = LookupDeclarations(node).TrySingle();
        if (node.IsAttributeType)
            symbolNode ??= LookupDeclarations(node, withAttributeSuffix: true).TrySingle();
        return symbolNode;
    }

    public static void StandardTypeName_ContributeDiagnostics(IStandardTypeNameNode node, Diagnostics diagnostics)
    {
        if (node.ReferencedDeclaration is not null)
            return;
        var symbolNodes = LookupDeclarations(node);
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

    private static IFixedSet<ITypeDeclarationNode> LookupDeclarations(IStandardTypeNameNode node, bool withAttributeSuffix = false)
    {
        var name = withAttributeSuffix ? node.Name + SpecialNames.AttributeSuffix : node.Name;
        return node.ContainingLexicalScope.Lookup(name).OfType<ITypeDeclarationNode>().ToFixedSet();
    }

    public static IFieldDefinitionNode? FieldParameter_ReferencedField(IFieldParameterNode node)
        // TODO report error for field parameter without referenced field
        => node.ContainingTypeDefinition.Members.OfType<IFieldDefinitionNode>().FirstOrDefault(f => f.Name == node.Name);

    #region Construct for Symbols
    public static IPackageFacetChildDeclarationNode Symbol(Symbol symbol)
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
