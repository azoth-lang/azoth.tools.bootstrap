using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class DefinitionPlainTypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<IMaybeNonVoidPlainType> InvocableDefinition_ParameterPlainTypes(IInvocableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeFunctionPlainType ConcreteFunctionInvocableDefinition_PlainType(IConcreteFunctionInvocableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial SelfTypeConstructor ImplicitSelfDefinition_TypeConstructor(IImplicitSelfDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidPlainType FieldDefinition_BindingPlainType(IFieldDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryTypeConstructor ClassDefinition_TypeConstructor(IClassDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryTypeConstructor StructDefinition_TypeConstructor(IStructDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryTypeConstructor TraitDefinition_TypeConstructor(ITraitDefinitionNode node);
}
