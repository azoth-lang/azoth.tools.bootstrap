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

    public static partial IMaybeFunctionPlainType ConcreteFunctionInvocableDefinition_PlainType(IConcreteFunctionInvocableDefinitionNode node)
        => FunctionPlainType.Create(node.ParameterPlainTypes, node.ReturnPlainType);
    #endregion

    #region Type Definitions
    public static partial OrdinaryTypeConstructor ClassDefinition_TypeFactory(IClassDefinitionNode node)
    {
        // TODO use ContainingTypeConstructor in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return TypeConstructor.CreateClass(node.Package.Name, containingNamespaceName,
            node.IsAbstract, node.IsConst, node.Name, GetGenericParameters(node), node.Supertypes);
    }

    public static partial OrdinaryTypeConstructor StructDefinition_TypeFactory(IStructDefinitionNode node)
    {
        // TODO use ContainingTypeConstructor in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return TypeConstructor.CreateStruct(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name, GetGenericParameters(node), node.Supertypes);
    }

    public static partial OrdinaryTypeConstructor TraitDefinition_TypeFactory(ITraitDefinitionNode node)
    {
        // TODO use ContainingTypeConstructor in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return TypeConstructor.CreateTrait(node.Package.Name, containingNamespaceName,
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
    public static partial SelfTypeConstructor ImplicitSelfDefinition_TypeFactory(IImplicitSelfDefinitionNode node)
        => new(node.ContainingDeclaration.TypeFactory);
    #endregion

    public static partial IMaybeNonVoidPlainType FieldDefinition_BindingPlainType(IFieldDefinitionNode node)
        => node.TypeNode.NamedPlainType.ToNonVoid();
}
