using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

/// <summary>
/// A factory to create symbol nodes from symbols.
/// </summary>
/// <remarks>This allows the semantic analysis to treat symbols loaded from referenced packages as
/// declarations in the semantic tree.</remarks>
internal static class SymbolBinder
{
    public static IChildDeclarationNode Symbol(Symbol symbol)
       => symbol switch
       {
           NamespaceSymbol sym => INamespaceSymbolNode.Create(sym),
           TypeSymbol sym => TypeSymbol(sym),
           InvocableSymbol sym => InvocableSymbol(sym),
           FieldSymbol sym => FieldSymbol(sym),
           _ => throw ExhaustiveMatch.Failed(symbol),
       };

    private static ITypeDeclarationNode TypeSymbol(TypeSymbol symbol)
        => symbol switch
        {
            OrdinaryTypeSymbol sym => UserTypeSymbol(sym),
            // These will be needed because the generic parameter type could be used in a type expression
            GenericParameterTypeSymbol sym => GenericParameterTypeSymbol(sym),
            EmptyTypeSymbol sym => EmptyTypeSymbol(sym),
            BuiltInTypeSymbol sym => PrimitiveTypeSymbol(sym),
            AssociatedTypeSymbol _ => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };

    private static IUserTypeDeclarationNode UserTypeSymbol(OrdinaryTypeSymbol symbol)
         => symbol.DeclaresType.Kind switch
         {
             TypeKind.Struct => IStructSymbolNode.Create(symbol),
             TypeKind.Class => IClassSymbolNode.Create(symbol),
             TypeKind.Trait => ITraitSymbolNode.Create(symbol),
             _ => throw ExhaustiveMatch.Failed(symbol.DeclaresType.Kind),
         };

    private static IEmptyTypeSymbolNode EmptyTypeSymbol(EmptyTypeSymbol sym)
        => IEmptyTypeSymbolNode.Create(sym);

    private static IPrimitiveTypeSymbolNode PrimitiveTypeSymbol(BuiltInTypeSymbol sym)
        => IPrimitiveTypeSymbolNode.Create(sym);

    private static IGenericParameterSymbolNode GenericParameterTypeSymbol(GenericParameterTypeSymbol sym)
        => IGenericParameterSymbolNode.Create(sym);

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
         => IFunctionSymbolNode.Create(sym);

    private static IConstructorDeclarationNode ConstructorSymbol(ConstructorSymbol sym)
        => IConstructorSymbolNode.Create(sym);

    private static IInitializerSymbolNode InitializerSymbol(InitializerSymbol sym)
        => IInitializerSymbolNode.Create(sym);

    private static IMethodDeclarationNode MethodSymbol(MethodSymbol sym)
        => sym.Kind switch
        {
            MethodKind.Standard => IStandardMethodSymbolNode.Create(sym),
            MethodKind.Getter => IGetterMethodSymbolNode.Create(sym),
            MethodKind.Setter => ISetterMethodSymbolNode.Create(sym),
            _ => throw ExhaustiveMatch.Failed(sym.Kind),
        };

    private static IFieldSymbolNode FieldSymbol(FieldSymbol sym)
        => IFieldSymbolNode.Create(sym);
}
