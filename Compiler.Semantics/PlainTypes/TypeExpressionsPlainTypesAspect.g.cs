using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeExpressionsPlainTypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType ViewpointType_NamedPlainType(IViewpointTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType OptionalType_NamedPlainType(IOptionalTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType CapabilityType_NamedPlainType(ICapabilityTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType CapabilitySetType_NamedPlainType(ICapabilitySetTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType FunctionType_NamedPlainType(IFunctionTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType BuiltInTypeName_NamedPlainType(IBuiltInTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType IdentifierTypeName_NamedPlainType(IIdentifierTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType GenericTypeName_NamedPlainType(IGenericTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType QualifiedTypeName_NamedPlainType(IQualifiedTypeNameNode node);
}
