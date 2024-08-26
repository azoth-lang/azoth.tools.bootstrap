using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

// ReSharper disable PartialTypeWithSinglePart
#nullable enable

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class OverloadResolutionAspect
{
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionGroupNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_MethodGroupNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_TypeNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_InitializerGroupNameExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_FunctionReferenceExpression(IUnresolvedInvocationExpressionNode node);
    public static partial IAmbiguousExpressionNode? UnresolvedInvocationExpression_Rewrite_ToUnknown(IUnresolvedInvocationExpressionNode node);
}
