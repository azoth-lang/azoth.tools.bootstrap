using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.TypeConstructors;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeConstructorsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial SelfTypeConstructor ImplicitSelfDefinition_TypeConstructor(IImplicitSelfDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial NamespaceContext CompilationUnit_TypeConstructorContext(ICompilationUnitNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial NamespaceContext NamespaceDefinition_TypeConstructorContext(INamespaceDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryTypeConstructor ClassDefinition_TypeConstructor(IClassDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryTypeConstructor StructDefinition_TypeConstructor(IStructDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryTypeConstructor TraitDefinition_TypeConstructor(ITraitDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryAssociatedTypeConstructor AssociatedTypeDefinition_TypeConstructor(IAssociatedTypeDefinitionNode node);
}
