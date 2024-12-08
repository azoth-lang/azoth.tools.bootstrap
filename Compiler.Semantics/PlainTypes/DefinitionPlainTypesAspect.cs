using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class DefinitionPlainTypesAspect
{
    public static partial IFixedList<IMaybeNonVoidPlainType> InvocableDefinition_ParameterPlainTypes(IInvocableDefinitionNode node)
        => node.Parameters.Select(p => p.BindingPlainType).ToFixedList();

    public static partial IMaybeFunctionPlainType ConcreteFunctionInvocableDefinition_PlainType(IConcreteFunctionInvocableDefinitionNode node)
        => FunctionPlainType.Create(node.ParameterPlainTypes, node.ReturnPlainType);

    public static partial OrdinaryTypeConstructor ClassDefinition_TypeConstructor(IClassDefinitionNode node)
    {
        // TODO use ContainingTypeConstructor in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return TypeConstructor.CreateClass(node.Package.Name, containingNamespaceName,
            node.IsAbstract, node.IsConst, node.Name, GetGenericParameters(node), node.Supertypes);
    }

    public static partial OrdinaryTypeConstructor StructDefinition_TypeConstructor(IStructDefinitionNode node)
    {
        // TODO use ContainingTypeConstructor in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return TypeConstructor.CreateStruct(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name, GetGenericParameters(node), node.Supertypes);
    }

    public static partial OrdinaryTypeConstructor TraitDefinition_TypeConstructor(ITraitDefinitionNode node)
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

    private static IFixedList<TypeConstructor.Parameter> GetGenericParameters(ITypeDefinitionNode node)
        => node.GenericParameters.Select(p => p.Parameter).ToFixedList();

    public static partial SelfPlainType TypeDefinition_SelfPlainType(ITypeDefinitionNode node)
        => new(node.TypeConstructor);

    public static partial IMaybeNonVoidPlainType FieldDefinition_BindingPlainType(IFieldDefinitionNode node)
        => node.TypeNode.NamedPlainType.ToNonVoid();
}
