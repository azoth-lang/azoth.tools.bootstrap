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
    public static partial IPackageReferenceNode Package_IntrinsicsReference(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeDeclarationNode> Package_PrimitivesDeclarations(IPackageNode node);
}
