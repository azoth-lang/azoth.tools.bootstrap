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
           PackageSymbol _ => throw new NotSupportedException($"{nameof(PackageSymbol)} not supported."),
           PackageFacetSymbol _ => throw new NotSupportedException($"{nameof(PackageFacetSymbol)} not supported."),
           LocalNamespaceSymbol sym => INamespaceSymbolNode.Create(sym),
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
            VoidTypeSymbol sym => VoidTypeSymbol(sym),
            NeverTypeSymbol sym => NeverTypeSymbol(sym),
            BuiltInTypeSymbol sym => PrimitiveTypeSymbol(sym),
            AssociatedTypeSymbol _ => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };

    private static IOrdinaryTypeDeclarationNode UserTypeSymbol(OrdinaryTypeSymbol symbol)
         => symbol.TypeConstructor.Kind switch
         {
             TypeKind.Class => IClassSymbolNode.Create(symbol),
             TypeKind.Struct => IStructSymbolNode.Create(symbol),
             TypeKind.Value => IValueSymbolNode.Create(symbol),
             TypeKind.Trait => ITraitSymbolNode.Create(symbol),
             _ => throw ExhaustiveMatch.Failed(symbol.TypeConstructor.Kind),
         };

    private static IVoidTypeSymbolNode VoidTypeSymbol(VoidTypeSymbol sym)
        => IVoidTypeSymbolNode.Create(sym);

    private static INeverTypeSymbolNode NeverTypeSymbol(NeverTypeSymbol sym)
        => INeverTypeSymbolNode.Create(sym);

    private static IBuiltInTypeSymbolNode PrimitiveTypeSymbol(BuiltInTypeSymbol sym)
        => IBuiltInTypeSymbolNode.Create(sym);

    private static IGenericParameterSymbolNode GenericParameterTypeSymbol(GenericParameterTypeSymbol sym)
        => IGenericParameterSymbolNode.Create(sym);

    private static IInvocableDeclarationNode InvocableSymbol(InvocableSymbol symbol)
        => symbol switch
        {
            FunctionSymbol sym => FunctionSymbol(sym),
            InitializerSymbol sym => InitializerSymbol(sym),
            MethodSymbol sym => MethodSymbol(sym),
            _ => throw ExhaustiveMatch.Failed(symbol),
        };

    private static IFunctionInvocableDeclarationNode FunctionSymbol(FunctionSymbol sym)
         => sym.ContextTypeSymbol is not null ? IAssociatedFunctionSymbolNode.Create(sym) : IFunctionSymbolNode.Create(sym);

    private static IInitializerSymbolNode InitializerSymbol(InitializerSymbol sym)
        => IInitializerSymbolNode.Create(sym);

    private static IMethodDeclarationNode MethodSymbol(MethodSymbol sym)
        => sym.Kind switch
        {
            MethodKind.Ordinary => IOrdinaryMethodSymbolNode.Create(sym),
            MethodKind.Getter => IGetterMethodSymbolNode.Create(sym),
            MethodKind.Setter => ISetterMethodSymbolNode.Create(sym),
            _ => throw ExhaustiveMatch.Failed(sym.Kind),
        };

    private static IFieldSymbolNode FieldSymbol(FieldSymbol sym)
        => IFieldSymbolNode.Create(sym);
}
