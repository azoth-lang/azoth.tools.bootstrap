using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeExpressionsAntetypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType ViewpointType_NamedPlainType(IViewpointTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType TypeNameExpression_NamedPlainType(ITypeNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType IdentifierTypeName_NamedPlainType(IIdentifierTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType SpecialTypeName_NamedPlainType(ISpecialTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType GenericTypeName_NamedPlainType(IGenericTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType OptionalType_NamedPlainType(IOptionalTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType CapabilityType_NamedPlainType(ICapabilityTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePlainType FunctionType_NamedPlainType(IFunctionTypeNode node);
}
