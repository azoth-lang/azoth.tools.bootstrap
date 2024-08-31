using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ControlFlowAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IEntryNode ExecutableDefinition_Entry(IExecutableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IExitNode ExecutableDefinition_Exit(IExecutableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ControlFlowSet ConcreteInvocableDefinition_Entry_ControlFlowFollowing(IConcreteInvocableDefinitionNode node);
}
