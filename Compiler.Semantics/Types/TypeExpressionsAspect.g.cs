using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeExpressionsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePseudotype? ConcreteMethodDefinition_Children_Broadcast_MethodSelfType(IConcreteMethodDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionType TypeName_NamedType(ITypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void CapabilityType_Contribute_Diagnostics(ICapabilityTypeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void CapabilityViewpointType_Contribute_Diagnostics(ICapabilityViewpointTypeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void SelfViewpointType_Contribute_Diagnostics(ISelfViewpointTypeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionType OptionalType_NamedType(IOptionalTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionType CapabilityType_NamedType(ICapabilityTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionType FunctionType_NamedType(IFunctionTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ParameterType ParameterType_Parameter(IParameterTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionType CapabilityViewpointType_NamedType(ICapabilityViewpointTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeExpressionType SelfViewpointType_NamedType(ISelfViewpointTypeNode node);
}
