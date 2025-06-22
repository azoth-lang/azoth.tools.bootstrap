using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class BuiltInsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeDeclarationNode> PackageFacet_PrimitivesDeclarations(IPackageFacetNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IPackageFacetReferenceNode PackageFacet_IntrinsicsReference(IPackageFacetNode node);
}
