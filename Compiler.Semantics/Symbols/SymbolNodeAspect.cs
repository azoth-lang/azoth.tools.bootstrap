using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static partial class SymbolNodeAspect
{
    public static partial INamespaceDefinitionNode PackageFacet_GlobalNamespace(IPackageFacetNode node)
    {
        var packageSymbol = node.PackageSymbol;
        var builder = new NamespaceDefinitionNodeBuilder(packageSymbol);
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

    public static partial INamespaceDefinitionNode CompilationUnit_ImplicitNamespace(ICompilationUnitNode node)
        => FindNamespace(node.ContainingDeclaration.GlobalNamespace, node.ImplicitNamespaceName);

    private static INamespaceDefinitionNode FindNamespace(INamespaceDefinitionNode containingDeclarationNode, NamespaceName ns)
    {
        // TODO rework
        var current = containingDeclarationNode;
        foreach (var name in ns.Segments)
            current = current.MembersNamed(name).OfType<INamespaceDefinitionNode>().Single();
        return current;
    }

    public static partial IPackageSymbolNode PackageReference_SymbolNode(IPackageReferenceNode node)
        => new PackageSymbolNode(node);

    public static partial FixedDictionary<IdentifierName, IPackageDeclarationNode> Package_PackageDeclarations(IPackageNode node)
        => node.References.Select(r => r.SymbolNode).Append<IPackageDeclarationNode>(node)
               .ToFixedDictionary(n => n.AliasOrName ?? node.Symbol.Name);

    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_ContainingNamespace(INamespaceBlockDefinitionNode node)
        => node.IsGlobalQualified ? node.Facet.GlobalNamespace : node.ContainingDeclaration;

    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_Definition(INamespaceBlockDefinitionNode node)
        => FindNamespace(node.ContainingNamespace, node.DeclaredNames);

    #region Type Symbol Nodes
    public static partial IFixedList<IGenericParameterSymbolNode> UserTypeSymbol_GenericParameters(IUserTypeSymbolNode node)
        => node.SymbolTree().GetChildrenOf(node.Symbol).OfType<GenericParameterTypeSymbol>()
               .Select(SymbolBinder.Symbol).WhereNotNull()
               .Cast<IGenericParameterSymbolNode>().ToFixedList();

    public static partial IFixedSet<ITypeMemberSymbolNode> BuiltInTypeSymbol_Members(IBuiltInTypeSymbolNode node)
        => GetMembers<ITypeMemberSymbolNode>(node);

    public static partial void Validate_ClassSymbolNode(UserTypeSymbol symbol)
        => Requires.That(symbol.DeclaresType is ObjectType { IsClass: true }, nameof(symbol),
            "Symbol must be for an class type.");

    public static partial IFixedSet<IClassMemberSymbolNode> ClassSymbol_Members(IClassSymbolNode node)
        => GetMembers<IClassMemberSymbolNode>(node);

    public static partial void Validate_StructSymbolNode(UserTypeSymbol symbol)
        => Requires.That(symbol.DeclaresType is StructType, nameof(symbol), "Symbol must be for a struct type.");

    public static partial IFixedSet<IStructMemberSymbolNode> StructSymbol_Members(IStructSymbolNode node)
        => GetMembers<IStructMemberSymbolNode>(node);

    public static partial void Validate_TraitSymbolNode(UserTypeSymbol symbol)
        => Requires.That(symbol.DeclaresType is ObjectType { IsClass: false }, nameof(symbol),
            "Symbol must be for an trait type.");

    public static partial IFixedSet<ITraitMemberSymbolNode> TraitSymbol_Members(ITraitSymbolNode node)
        => GetMembers<ITraitMemberSymbolNode>(node);

    private static IFixedSet<T> GetMembers<T>(ITypeSymbolNode node)
        where T : IChildDeclarationNode
        => node.SymbolTree().GetChildrenOf(node.Symbol).Where(sym => sym is not GenericParameterTypeSymbol)
               .Select(SymbolBinder.Symbol).WhereNotNull().OfType<T>().ToFixedSet();
    #endregion

    public static partial ITypeDeclarationNode? StandardTypeName_ReferencedDeclaration(IStandardTypeNameNode node)
    {
        var symbolNode = LookupDeclarations(node).TrySingle();
        if (node.IsAttributeType)
            symbolNode ??= LookupDeclarations(node, withAttributeSuffix: true).TrySingle();
        return symbolNode;
    }

    public static partial void StandardTypeName_Contribute_Diagnostics(IStandardTypeNameNode node, DiagnosticCollectionBuilder diagnostics)
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

    public static partial IFieldDefinitionNode? FieldParameter_ReferencedField(IFieldParameterNode node)
        // TODO report error for field parameter without referenced field
        => node.ContainingTypeDefinition.Members.OfType<IFieldDefinitionNode>().FirstOrDefault(f => f.Name == node.Name);

    #region Symbol Nodes
    public static partial INamespaceSymbolNode PackageFacetSymbol_GlobalNamespace(IPackageFacetSymbolNode node)
        => new NamespaceSymbolNode(node.SymbolTree.Package);
    #endregion
}
