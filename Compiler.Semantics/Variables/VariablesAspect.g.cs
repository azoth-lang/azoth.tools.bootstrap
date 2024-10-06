using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class VariablesAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<IVariableBindingNode, int> InvocableDefinition_VariableBindingsMap(IInvocableDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<IVariableBindingNode, int> FieldDefinition_VariableBindingsMap(IFieldDefinitionNode node);
}
