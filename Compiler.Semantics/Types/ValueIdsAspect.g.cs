using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

// ReSharper disable PartialTypeWithSinglePart
#nullable enable

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class ValueIdsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ValueIdScope InvocableDefinition_ValueIdScope(IInvocableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ValueId Parameter_BindingValueId(IParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ValueId AmbiguousExpression_ValueId(IAmbiguousExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ValueId FieldDefinition_BindingValueId(IFieldDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ValueIdScope FieldDefinition_ValueIdScope(IFieldDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ValueId VariableDeclarationStatement_BindingValueId(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ValueId BindingPattern_BindingValueId(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ValueId ForeachExpression_BindingValueId(IForeachExpressionNode node);
}