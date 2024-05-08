using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class DeclaredTypeAttribute
{
    public static ObjectType ClassDeclaration_DeclaredType(IClassDeclarationNode node)
    {
        var packageName = node.Package.Name;
        var genericParameters = GetGenericParameters(node);

        // TODO properly handle super types
        var superTypes = new Lazy<IFixedSet<BareReferenceType>>(FixedSet.Empty<BareReferenceType>());

        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);

        var classType = ObjectType.CreateClass(packageName, containingNamespaceName, node.IsAbstract, node.IsConst,
            node.Name, genericParameters, superTypes);
        return classType;
    }

    public static StructType StructDeclaration_DeclaredType(IStructDeclarationNode node)
    {
        var packageName = node.Package.Name;
        var genericParameters = GetGenericParameters(node);

        // TODO properly handle super types
        var superTypes = new Lazy<IFixedSet<BareReferenceType>>(FixedSet.Empty<BareReferenceType>());
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        var structType = StructType.Create(packageName, containingNamespaceName, node.IsConst, node.Name,
            genericParameters, superTypes);

        return structType;
    }

    public static ObjectType TraitDeclaration_DeclaredType(ITraitDeclarationNode node)
    {
        var packageName = node.Package.Name;
        var genericParameters = GetGenericParameters(node);

        // TODO properly handle super types
        var superTypes = new Lazy<IFixedSet<BareReferenceType>>(FixedSet.Empty<BareReferenceType>());
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        var traitType = ObjectType.CreateTrait(packageName, containingNamespaceName, node.IsConst, node.Name,
            genericParameters, superTypes);

        return traitType;
    }

    private static NamespaceName GetContainingNamespaceName(ITypeMemberDeclarationNode node)
    {
        // TODO correctly deal with containing namespace
        var containingSymbol = node.ContainingSymbol;
        while (containingSymbol is not NamespaceSymbol) containingSymbol = containingSymbol.ContainingSymbol!;
        var containingNamespaceName = ((NamespaceSymbol)containingSymbol).NamespaceName;
        return containingNamespaceName;
    }

    private static IFixedList<GenericParameter> GetGenericParameters(ITypeDeclarationNode node)
        => node.GenericParameters.Select(p => p.Parameter).ToFixedList();

    public static GenericParameterType GenericParameter_DeclaredType(IGenericParameterNode node)
        => node.ContainingDeclaredType.GenericParameterTypes.Single(t => t.Parameter == node.Parameter);
}
