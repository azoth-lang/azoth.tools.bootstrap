using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class BareTypeAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ConstructedPlainType TypeDefinition_BareSelfType(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BareType? TypeNameExpression_NamedBareType(ITypeNameExpressionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BareType? IdentifierTypeName_NamedBareType(IIdentifierTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BareType? BuiltInTypeName_NamedBareType(IBuiltInTypeNameNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial BareType? GenericTypeName_NamedBareType(IGenericTypeNameNode node);
}
