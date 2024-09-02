using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class NameBindingAntetypesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype SelfParameter_BindingAntetype(ISelfParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void VariableDeclarationStatement_Contribute_Diagnostics(IVariableDeclarationStatementNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype BindingContextPattern_Pattern_ContextBindingAntetype(IBindingContextPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype OptionalPattern_Pattern_ContextBindingAntetype(IOptionalPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype PatternMatchExpression_Pattern_ContextBindingAntetype(IPatternMatchExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype NamedParameter_BindingAntetype(INamedParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype FieldParameter_BindingAntetype(IFieldParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype VariableDeclarationStatement_BindingAntetype(IVariableDeclarationStatementNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype BindingPattern_BindingAntetype(IBindingPatternNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype NewObjectExpression_ConstructingAntetype(INewObjectExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IMaybeAntetype ForeachExpression_BindingAntetype(IForeachExpressionNode node);
}
