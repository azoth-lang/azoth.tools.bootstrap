using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ShadowingAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void VariableBinding_Contribute_Diagnostics(IVariableBindingNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void VariableNameExpression_Contribute_Diagnostics(IVariableNameExpressionNode node, DiagnosticCollectionBuilder diagnostics);
}
