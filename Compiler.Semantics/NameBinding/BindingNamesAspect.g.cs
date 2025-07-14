using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class BindingNamesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void Attribute_Contribute_Diagnostics(IAttributeNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ISelfParameterNode? InstanceExpression_ReferencedDefinition(IInstanceExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void SelfExpression_Contribute_Diagnostics(ISelfExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void BaseExpression_Contribute_Diagnostics(IBaseExpressionNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ITypeDeclarationNode? OrdinaryTypeName_ReferencedDeclaration(IOrdinaryTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void OrdinaryTypeName_Contribute_Diagnostics(IOrdinaryTypeNameNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFieldDefinitionNode? FieldParameter_ReferencedField(IFieldParameterNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ITypeDeclarationNode? BuiltInTypeName_ReferencedDeclaration(IBuiltInTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ITypeDeclarationNode? QualifiedTypeName_ReferencedDeclaration(IQualifiedTypeNameNode node);
}
