using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class NameLookupAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>> NamespaceDeclaration_MembersByName(INamespaceDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>> NamespaceDeclaration_NestedMembersByName(INamespaceDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> BuiltInTypeDeclaration_InclusiveInstanceMembersByName(IBuiltInTypeDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> BuiltInTypeDeclaration_AssociatedMembersByName(IBuiltInTypeDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>> OrdinaryTypeDeclaration_InclusiveInstanceMembersByName(IOrdinaryTypeDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>> OrdinaryTypeDeclaration_AssociatedMembersByName(IOrdinaryTypeDeclarationNode node);
}
