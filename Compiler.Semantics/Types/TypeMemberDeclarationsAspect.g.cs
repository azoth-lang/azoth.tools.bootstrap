using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeMemberDeclarationsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState ConcreteInvocableDefinition_FlowStateBefore(IConcreteInvocableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodDefinition_Contribute_Diagnostics(IMethodDefinitionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FunctionType FunctionDefinition_Type(IFunctionDefinitionNode node);
}
