using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeDefinitionsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<BareReferenceType> TypeDefinition_Supertypes(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void TypeDefinition_Contribute_Diagnostics(ITypeDefinitionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void ClassDefinition_Contribute_Diagnostics(IClassDefinitionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ObjectType ClassDefinition_DeclaredType(IClassDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial StructType StructDefinition_DeclaredType(IStructDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ObjectType TraitDefinition_DeclaredType(ITraitDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial GenericParameter GenericParameter_Parameter(IGenericParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial GenericParameterType GenericParameter_DeclaredType(IGenericParameterNode node);
}
