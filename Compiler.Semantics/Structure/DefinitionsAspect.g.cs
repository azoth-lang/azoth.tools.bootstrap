using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class DefinitionsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial PackageSymbol PackageFacet_PackageSymbol(IPackageFacetNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial void CompilationUnit_Contribute_Diagnostics(ICompilationUnitNode node, DiagnosticCollectionBuilder diagnostics);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFunctionDefinitionNode? Package_EntryPoint(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IFacetMemberDefinitionNode> PackageFacet_Definitions(IPackageFacetNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<INamespaceMemberDefinitionNode> NamespaceDefinition_Members(INamespaceDefinitionNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial TypeVariance AssociatedTypeDefinition_Variance(IAssociatedTypeDefinitionNode node);
}
