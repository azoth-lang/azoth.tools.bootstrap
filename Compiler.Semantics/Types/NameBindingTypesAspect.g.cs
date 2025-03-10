using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class NameBindingTypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void NamedParameter_Contribute_Diagnostics(INamedParameterNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType SelfParameter_ParameterType(ISelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void InitializerSelfParameter_Contribute_Diagnostics(IInitializerSelfParameterNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodSelfParameter_Contribute_Diagnostics(IMethodSelfParameterNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState VariableDeclarationStatement_FlowStateAfter(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType BindingContextPattern_Pattern_ContextBindingType(IBindingContextPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFlowState BindingPattern_FlowStateAfter(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType OptionalPattern_Pattern_ContextBindingType(IOptionalPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType PatternMatchExpression_Pattern_ContextBindingType(IPatternMatchExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType FieldDefinition_BindingType(IFieldDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeParameterType NamedParameter_ParameterType(INamedParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType NamedParameter_BindingType(INamedParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial CapabilityType InitializerSelfParameter_BindingType(IInitializerSelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType MethodSelfParameter_BindingType(IMethodSelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType FieldParameter_BindingType(IFieldParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeParameterType FieldParameter_ParameterType(IFieldParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType VariableDeclarationStatement_BindingType(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType BindingPattern_BindingType(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeNonVoidType ForeachExpression_BindingType(IForeachExpressionNode node);
}
