using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class DefiniteAssignmentAspect
{
    public static partial BindingFlags<IVariableBindingNode> DataFlow_DefinitelyAssigned_Initial(IDataFlowNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void VariableNameExpression_Contribute_Diagnostics(IVariableNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> Entry_DefinitelyAssigned(IEntryNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> Exit_DefinitelyAssigned(IExitNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> VariableDeclarationStatement_DefinitelyAssigned(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> BindingPattern_DefinitelyAssigned(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> AssignmentExpression_DefinitelyAssigned(IAssignmentExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BindingFlags<IVariableBindingNode> ForeachExpression_DefinitelyAssigned(IForeachExpressionNode node);
}
