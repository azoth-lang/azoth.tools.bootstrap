using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class SynthesizedMembersAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberDefinitionNode> TypeDefinition_DeclaredMembers(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IImplicitSelfDefinitionNode TypeDefinition_ImplicitSelf(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberDefinitionNode> ClassDefinition_SynthesizedMembers(IClassDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IDefaultInitializerDefinitionNode? ClassDefinition_DefaultInitializer(IClassDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberDefinitionNode> StructDefinition_SynthesizedMembers(IStructDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IDefaultInitializerDefinitionNode? StructDefinition_DefaultInitializer(IStructDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeMemberDefinitionNode> ValueDefinition_SynthesizedMembers(IValueDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IDefaultInitializerDefinitionNode? ValueDefinition_DefaultInitializer(IValueDefinitionNode node);
}
