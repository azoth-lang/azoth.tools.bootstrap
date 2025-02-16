using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class TypeModifiersAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial AccessModifier FunctionDefinition_AccessModifier(IFunctionDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial AccessModifier TypeDefinition_AccessModifier(ITypeDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial AccessModifier TypeMemberDefinition_AccessModifier(ITypeMemberDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void MethodDefinition_Contribute_Diagnostics(IMethodDefinitionNode node, DiagnosticCollectionBuilder diagnostics);
}
