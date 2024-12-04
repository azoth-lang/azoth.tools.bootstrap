using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class DefinitionPlainTypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeFunctionPlainType ConcreteFunctionInvocableDefinition_PlainType(IConcreteFunctionInvocableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial OrdinaryTypeConstructor TypeDefinition_TypeConstructor(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial SelfPlainType TypeDefinition_SelfPlainType(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidPlainType FieldDefinition_BindingPlainType(IFieldDefinitionNode node);
}
