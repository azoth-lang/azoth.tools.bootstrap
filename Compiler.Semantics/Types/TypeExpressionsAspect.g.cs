using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeExpressionsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybePseudotype? MethodDefinition_Children_Broadcast_MethodSelfType(IMethodDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType TypeName_NamedType(ITypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void CapabilityType_Contribute_Diagnostics(ICapabilityTypeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void CapabilityViewpointType_Contribute_Diagnostics(ICapabilityViewpointTypeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void SelfViewpointType_Contribute_Diagnostics(ISelfViewpointTypeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType OptionalType_NamedType(IOptionalTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType CapabilityType_NamedType(ICapabilityTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType FunctionType_NamedType(IFunctionTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeParameterType ParameterType_Parameter(IParameterTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType CapabilityViewpointType_NamedType(ICapabilityViewpointTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeType SelfViewpointType_NamedType(ISelfViewpointTypeNode node);
}
