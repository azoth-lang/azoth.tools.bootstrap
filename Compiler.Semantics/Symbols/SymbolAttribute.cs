using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
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

    public static TypeSymbol? IdentifierTypeName(IIdentifierTypeNameNode node)
        => node.ReferencedSymbolNode?.Symbol;

    public static TypeSymbol? GenericTypeName(IGenericTypeNameNode node)
        => node.ReferencedSymbolNode?.Symbol;

    public static TypeSymbol SpecialTypeName(ISpecialTypeNameNode node)
        => Primitive.SymbolTree.LookupSymbol(node.Name);

    public static FunctionSymbol FunctionDeclaration(IFunctionDeclarationNode node)
        => new(node.ContainingSymbol, node.Name, node.Type);

    public static MethodSymbol MethodDeclaration(IMethodDeclarationNode node)
        => new MethodSymbol(node.ContainingSymbol, node.Kind, node.Name,
            node.SelfParameter.ParameterType, node.Parameters.Select(p => p.Parameter).ToFixedList(),
            new Return(node.Return?.Type ?? DataType.Void));
}
