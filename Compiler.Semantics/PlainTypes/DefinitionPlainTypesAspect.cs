using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class DefinitionPlainTypesAspect
{
    #region Definitions
    public static partial IFixedList<IMaybeNonVoidPlainType> InvocableDefinition_ParameterPlainTypes(IInvocableDefinitionNode node)
        => node.Parameters.Select(p => p.BindingPlainType).ToFixedList();

    public static partial IMaybeFunctionPlainType FunctionInvocableDefinition_PlainType(IFunctionInvocableDefinitionNode node)
        => FunctionPlainType.Create(node.ParameterPlainTypes, node.ReturnPlainType);
    #endregion

    #region Type Definitions
    public static partial OrdinaryTypeConstructor ClassDefinition_TypeConstructor(IClassDefinitionNode node)
    {
        // TODO use ContainingTypeConstructor in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return BareTypeConstructor.CreateClass(node.Package.Name, containingNamespaceName,
            node.IsAbstract, node.IsConst, node.Name, GetGenericParameters(node), node.Supertypes);
    }

    public static partial OrdinaryTypeConstructor StructDefinition_TypeConstructor(IStructDefinitionNode node)
    {
        // TODO use ContainingTypeConstructor in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return BareTypeConstructor.CreateStruct(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name, GetGenericParameters(node), node.Supertypes);
    }

    public static partial OrdinaryTypeConstructor TraitDefinition_TypeConstructor(ITraitDefinitionNode node)
    {
        // TODO use ContainingTypeConstructor in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return BareTypeConstructor.CreateTrait(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name, GetGenericParameters(node), node.Supertypes);
    }

    private static NamespaceName GetContainingNamespaceName(ITypeMemberDefinitionNode node)
    {
        // TODO correctly deal with containing namespace
        var containingSymbol = node.ContainingSymbol!; // Since it is a type or namespace, it will have a symbol
        while (containingSymbol is not NamespaceSymbol) containingSymbol = containingSymbol.ContainingSymbol!;
        var containingNamespaceName = ((NamespaceSymbol)containingSymbol).NamespaceName;
        return containingNamespaceName;
    }

    private static IFixedList<TypeConstructorParameter> GetGenericParameters(ITypeDefinitionNode node)
        => node.GenericParameters.Select(p => p.Parameter).ToFixedList();
    #endregion

    #region Type Definition Parts
    public static partial SelfTypeConstructor ImplicitSelfDefinition_TypeConstructor(IImplicitSelfDefinitionNode node)
        => new(node.ContainingDeclaration.TypeConstructor);
    #endregion

    #region Member Definitions
    public static partial IMaybeNonVoidPlainType FieldDefinition_BindingPlainType(IFieldDefinitionNode node)
        => node.TypeNode.NamedPlainType.ToNonVoid();

    public static partial OrdinaryAssociatedTypeConstructor AssociatedTypeDefinition_TypeConstructor(IAssociatedTypeDefinitionNode node)
        => new(node.ContainingDeclaration.TypeConstructor, node.Name);
    #endregion
}
