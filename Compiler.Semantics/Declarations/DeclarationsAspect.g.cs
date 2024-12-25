using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Declarations;

#nullable enable
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
internal static partial class DeclarationsAspect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial IFixedList<INamespaceMemberDeclarationNode> NamespaceDeclaration_NestedMembers(INamespaceDeclarationNode node);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static partial ITypeConstructor GenericParameterDeclaration_TypeConstructor(IGenericParameterDeclarationNode node);
}
