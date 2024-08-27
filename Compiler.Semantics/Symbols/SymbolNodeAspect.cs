using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
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

    public static partial INamespaceDefinitionNode CompilationUnit_Children_ContainingDeclaration(ICompilationUnitNode node)
        => node.ImplicitNamespace;

    public static partial IPackageSymbolNode PackageReference_SymbolNode(IPackageReferenceNode node)
        => new PackageSymbolNode(node);

    public static partial FixedDictionary<IdentifierName, IPackageDeclarationNode> Package_PackageDeclarations(IPackageNode node)
        => node.References.Select(r => r.SymbolNode).Append<IPackageDeclarationNode>(node)
               .ToFixedDictionary(n => n.AliasOrName ?? node.Symbol.Name);

    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_ContainingNamespace(INamespaceBlockDefinitionNode node)
        => node.IsGlobalQualified ? node.Facet.GlobalNamespace : node.ContainingDeclaration;

    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_Definition(INamespaceBlockDefinitionNode node)
        => FindNamespace(node.ContainingNamespace, node.DeclaredNames);

    public static INamespaceDeclarationNode NamespaceBlockDefinition_Children_Broadcast_ContainingDeclaration(INamespaceBlockDefinitionNode node)
        => node.Definition;

    public static bool Attribute_TypeName_IsAttributeType(IAttributeNode _) => true;

    public static IUserTypeDeclarationNode TypeDefinition_Children_Broadcast_ContainingDeclaration(ITypeDefinitionNode node)
        => node;

    public static bool TypeDefinition_Children_Broadcast_IsAttributeType(ITypeDefinitionNode _)
        => false;

    public static bool FunctionDefinition_Children_Broadcast_IsAttributeType(IFunctionDefinitionNode _)
        => false;

    public static partial ITypeDeclarationNode? StandardTypeName_ReferencedDeclaration(IStandardTypeNameNode node)
    {
        var symbolNode = LookupDeclarations(node).TrySingle();
        if (node.IsAttributeType)
            symbolNode ??= LookupDeclarations(node, withAttributeSuffix: true).TrySingle();
        return symbolNode;
    }

    public static void StandardTypeName_ContributeDiagnostics(IStandardTypeNameNode node, DiagnosticCollectionBuilder diagnostics)
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

    #region Construct for Symbols
    public static IChildDeclarationNode Symbol(Symbol symbol)
        => symbol switch
        {
            NamespaceSymbol sym => new NamespaceSymbolNode(sym),
            TypeSymbol sym => TypeSymbol(sym),
            InvocableSymbol sym => InvocableSymbol(sym),
            FieldSymbol sym => FieldSymbol(sym),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };
    private static ITypeDeclarationNode TypeSymbol(TypeSymbol symbol)
        => symbol switch
        {
            UserTypeSymbol sym => UserTypeSymbol(sym),
            // These will be needed because the generic parameter type could be used in a type expression
            GenericParameterTypeSymbol sym => GenericParameterTypeSymbol(sym),
            EmptyTypeSymbol sym => EmptyTypeSymbol(sym),
            PrimitiveTypeSymbol sym => PrimitiveTypeSymbol(sym),
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

    private static IPrimitiveTypeSymbolNode EmptyTypeSymbol(EmptyTypeSymbol sym)
        => new EmptyTypeSymbolNode(sym);

    private static IPrimitiveTypeSymbolNode PrimitiveTypeSymbol(PrimitiveTypeSymbol sym)
        => new PrimitiveTypeSymbolNode(sym);

    private static IGenericParameterSymbolNode GenericParameterTypeSymbol(GenericParameterTypeSymbol sym)
        => new GenericParameterSymbolNode(sym);

    private static IPackageFacetChildDeclarationNode InvocableSymbol(InvocableSymbol symbol)
        => symbol switch
        {
            FunctionSymbol sym => FunctionSymbol(sym),
            ConstructorSymbol sym => ConstructorSymbol(sym),
            InitializerSymbol sym => InitializerSymbol(sym),
            MethodSymbol sym => MethodSymbol(sym),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };

    private static IFunctionDeclarationNode FunctionSymbol(FunctionSymbol sym)
         => new FunctionSymbolNode(sym);

    private static IConstructorDeclarationNode ConstructorSymbol(ConstructorSymbol sym)
        => new ConstructorSymbolNode(sym);

    private static IInitializerSymbolNode InitializerSymbol(InitializerSymbol sym)
        => new InitializerSymbolNode(sym);

    private static IMethodDeclarationNode MethodSymbol(MethodSymbol sym)
        => sym.Kind switch
        {
            MethodKind.Standard => new StandardMethodSymbolNode(sym),
            MethodKind.Getter => new GetterMethodSymbolNode(sym),
            MethodKind.Setter => new SetterMethodSymbolNode(sym),
            _ => throw ExhaustiveMatch.Failed(sym.Kind),
        };

    private static IFieldSymbolNode FieldSymbol(FieldSymbol sym)
        => new FieldSymbolNode(sym);
    #endregion
}
