using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

// ReSharper disable PartialTypeWithSinglePart
#nullable enable

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class BuiltInsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IPackageReferenceNode Package_IntrinsicsReference(IPackageNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedSet<ITypeDeclarationNode> Package_PrimitivesDeclarations(IPackageNode node);
}
