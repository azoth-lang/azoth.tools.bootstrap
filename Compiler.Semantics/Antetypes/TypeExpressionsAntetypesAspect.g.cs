using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeExpressionsAntetypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype ViewpointType_NamedAntetype(IViewpointTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype TypeNameExpression_NamedAntetype(ITypeNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype IdentifierTypeName_NamedAntetype(IIdentifierTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype SpecialTypeName_NamedAntetype(ISpecialTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype GenericTypeName_NamedAntetype(IGenericTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype OptionalType_NamedAntetype(IOptionalTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype CapabilityType_NamedAntetype(ICapabilityTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype FunctionType_NamedAntetype(IFunctionTypeNode node);
}
