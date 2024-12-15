using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class SymbolsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial PackageSymbol Package_Symbol(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FunctionSymbol? FunctionDefinition_Symbol(IFunctionDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryTypeSymbol TypeDefinition_Symbol(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial GenericParameterTypeSymbol GenericParameter_Symbol(IGenericParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial MethodSymbol? MethodDefinition_Symbol(IMethodDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConstructorSymbol? DefaultConstructorDefinition_Symbol(IDefaultConstructorDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConstructorSymbol? SourceConstructorDefinition_Symbol(ISourceConstructorDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial InitializerSymbol? DefaultInitializerDefinition_Symbol(IDefaultInitializerDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial InitializerSymbol? SourceInitializerDefinition_Symbol(ISourceInitializerDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FieldSymbol? FieldDefinition_Symbol(IFieldDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FunctionSymbol? AssociatedFunctionDefinition_Symbol(IAssociatedFunctionDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IPackageSymbols Package_PackageSymbols(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConstructorSymbol? Attribute_ReferencedSymbol(IAttributeNode node);
}
