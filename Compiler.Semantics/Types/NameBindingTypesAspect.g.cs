using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class NameBindingTypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType FieldDefinition_BindingType(IFieldDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType NamedParameter_BindingType(INamedParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial CapabilityType ConstructorSelfParameter_BindingType(IConstructorSelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial CapabilityType InitializerSelfParameter_BindingType(IInitializerSelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial Pseudotype MethodSelfParameter_BindingType(IMethodSelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType FieldParameter_BindingType(IFieldParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType VariableDeclarationStatement_BindingType(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType BindingPattern_BindingType(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial DataType ForeachExpression_BindingType(IForeachExpressionNode node);
}
