using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

// ReSharper disable PartialTypeWithSinglePart
#nullable enable

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class DefinitionsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFunctionDefinitionNode? Package_EntryPoint(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<IPackageMemberDefinitionNode> PackageFacet_Definitions(IPackageFacetNode node);
}
