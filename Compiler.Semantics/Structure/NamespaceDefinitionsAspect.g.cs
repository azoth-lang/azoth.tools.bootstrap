using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class NamespaceDefinitionsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode PackageFacet_GlobalNamespace(IPackageFacetNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode CompilationUnit_ImplicitNamespace(ICompilationUnitNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_ContainingNamespace(INamespaceBlockDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_Definition(INamespaceBlockDefinitionNode node);
}
