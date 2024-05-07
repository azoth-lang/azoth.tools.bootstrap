using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolAttribute
{
    public static PackageSymbol Package(IPackageNode node) => new PackageSymbol(node.Name);

    public static UserTypeSymbol Class(IClassDeclarationNode node)
    {
        var classSymbol = new UserTypeSymbol(node.ContainingSymbol, node.Type);
        return classSymbol;
        //var classSymbol = new UserTypeSymbol(@class.ContainingNamespaceSymbol, classType);
        //@class.Symbol.Fulfill(classSymbol);

        //BuildSupertypes(@class, superTypes, typeDeclarations);

        //symbolTree.Add(classSymbol);

        //symbolTree.Add(genericParameterSymbols);
        //@class.CreateDefaultConstructor(symbolTree);
        //return;
    }

    public static GenericParameterTypeSymbol GenericParameter(IGenericParameterNode node)
        // TODO deal with cycle that was handled by promise
        => new GenericParameterTypeSymbol(Promise.ForValue(((ITypeDeclarationNode)node.Parent).Symbol), node.Type);

    public static TypeSymbol? IdentifierTypeName(IIdentifierTypeNameNode node)
        => node.ReferencedSymbolNode?.Symbol;

    public static TypeSymbol? GenericTypeName(IGenericTypeNameNode node)
        => node.ReferencedSymbolNode?.Symbol;

    public static TypeSymbol SpecialTypeName(ISpecialTypeNameNode node)
        => Primitive.SymbolTree.LookupSymbol(node.Name);
}
