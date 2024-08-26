using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

// ReSharper disable PartialTypeWithSinglePart
#nullable enable

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class NameLookupAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> NamespaceDeclaration_MembersByName(INamespaceDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> NamespaceDeclaration_NestedMembersByName(INamespaceDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> PrimitiveTypeDeclaration_InclusiveInstanceMembersByName(IPrimitiveTypeDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> PrimitiveTypeDeclaration_AssociatedMembersByName(IPrimitiveTypeDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>> UserTypeDeclaration_InclusiveInstanceMembersByName(IUserTypeDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>> UserTypeDeclaration_AssociatedMembersByName(IUserTypeDeclarationNode node);
}
