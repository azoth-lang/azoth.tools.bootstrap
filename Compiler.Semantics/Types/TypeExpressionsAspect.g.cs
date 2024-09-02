using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeExpressionsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial Pseudotype? ConcreteMethodDefinition_Children_Broadcast_MethodSelfType(IConcreteMethodDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType TypeName_NamedType(ITypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void CapabilityType_Contribute_Diagnostics(ICapabilityTypeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void SelfViewpointType_Contribute_Diagnostics(ISelfViewpointTypeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType OptionalType_NamedType(IOptionalTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType CapabilityType_NamedType(ICapabilityTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType FunctionType_NamedType(IFunctionTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType CapabilityViewpointType_NamedType(ICapabilityViewpointTypeNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType SelfViewpointType_NamedType(ISelfViewpointTypeNode node);
}
