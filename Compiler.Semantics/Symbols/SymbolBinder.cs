using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolBinder
{
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
}
