using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolAttribute
{
    public static PackageSymbol Package(IPackageNode node) => new(node.Name);

    public static UserTypeSymbol TypeDeclaration(ITypeDeclarationNode node)
        => new(node.ContainingSymbol, node.DeclaredType);

    public static GenericParameterTypeSymbol GenericParameter(IGenericParameterNode node)
        => new(node.ContainingSymbol, node.DeclaredType);

    public static TypeSymbol? StandardTypeName(IStandardTypeNameNode node)
        => node.ReferencedSymbolNode?.Symbol;

    public static TypeSymbol SpecialTypeName_ReferencedSymbol(ISpecialTypeNameNode node)
        => Primitive.SymbolTree.LookupSymbol(node.Name);

    public static FunctionSymbol FunctionDeclaration(IFunctionDeclarationNode node)
        => new(node.ContainingSymbol, node.Name, node.Type);

    public static MethodSymbol MethodDeclaration(IMethodDeclarationNode node)
        => new MethodSymbol(node.ContainingSymbol, node.Kind, node.Name,
            node.SelfParameter.ParameterType,
            node.Parameters.Select(p => p.ParameterType).ToFixedList(),
            new(node.Return?.Type ?? DataType.Void));

    public static ConstructorSymbol ConstructorDeclaration(IConstructorDeclarationNode node)
        => new ConstructorSymbol(node.ContainingSymbol, node.Name, node.SelfParameter.Type,
            node.Parameters.Select(p => p.ParameterType).ToFixedList());

    public static FieldSymbol FieldDeclaration(IFieldDeclarationNode node)
        => new FieldSymbol(node.ContainingSymbol, node.Name, node.IsMutableBinding, node.Type);

    public static InitializerSymbol InitializerDeclaration(IInitializerDeclarationNode node)
        => new InitializerSymbol(node.ContainingSymbol, node.Name, node.SelfParameter.Type,
            node.Parameters.Select(p => p.ParameterType).ToFixedList());

    public static FunctionSymbol AssociatedFunctionDeclaration(IAssociatedFunctionDeclarationNode node)
        => new(node.ContainingSymbol, node.Name, node.Type);

    public static ConstructorSymbol? Attribute_ReferencedSymbol(IAttributeNode node)
    {
        var referencedTypeSymbolNode = node.TypeName.ReferencedSymbolNode;
        if (referencedTypeSymbolNode is not IUserTypeSymbolNode userTypeSymbolNode)
            return null;

        return userTypeSymbolNode.Members.OfType<IConstructorSymbolNode>().Select(c => c.Symbol)
                                 .SingleOrDefault(s => s.Arity == 0);
    }

    public static void Attribute_ContributeDiagnostics(IAttributeNode node, Diagnostics diagnostics)
    {
        if (node.ReferencedSymbol is null)
            diagnostics.Add(NameBindingError.CouldNotBindName(node.File, node.TypeName.Syntax.Span));
    }

    public static ConstructorSymbol? ClassDeclaration_DefaultConstructorSymbol(IClassDeclarationNode node)
    {
        if (node.Members.Any(m => m is IConstructorDeclarationNode))
            return null;

        return ConstructorSymbol.CreateDefault(node.Symbol);
    }
}
