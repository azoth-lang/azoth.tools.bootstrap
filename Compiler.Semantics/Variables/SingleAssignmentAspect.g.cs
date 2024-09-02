using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class SingleAssignmentAspect
{
    public static partial BindingFlags<IVariableBindingNode> DataFlow_DefinitelyUnassigned_Initial(IDataFlowNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void VariableNameExpression_Contribute_Diagnostics(IVariableNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> Entry_DefinitelyUnassigned(IEntryNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> Exit_DefinitelyUnassigned(IExitNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> VariableDeclarationStatement_DefinitelyUnassigned(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> BindingPattern_DefinitelyUnassigned(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> AssignmentExpression_DefinitelyUnassigned(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> ForeachExpression_DefinitelyUnassigned(IForeachExpressionNode node);
}
