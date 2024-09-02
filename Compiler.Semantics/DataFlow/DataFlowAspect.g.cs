using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class DataFlowAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IDataFlowNode> DataFlow_DataFlowPrevious(IDataFlowNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IDataFlowNode> VariableNameExpression_DataFlowPrevious(IVariableNameExpressionNode node);
}
