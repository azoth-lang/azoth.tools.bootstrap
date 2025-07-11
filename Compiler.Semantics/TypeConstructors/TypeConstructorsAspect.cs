using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.TypeConstructors;

internal static partial class TypeConstructorsAspect
{
    #region Code Files
    public static partial NamespaceContext CompilationUnit_TypeConstructorContext(ICompilationUnitNode node)
        => new(node.PackageSymbol.Name, node.ImplicitNamespaceName);
    #endregion

    #region Namespace Definitions
    public static partial NamespaceContext NamespaceDefinition_TypeConstructorContext(INamespaceDefinitionNode node)
        => new(node.PackageSymbol.Name, node.NamespaceName);
    #endregion

    #region Type Definitions
    public static partial OrdinaryTypeConstructor ClassDefinition_TypeConstructor(IClassDefinitionNode node)
        => BareTypeConstructor.CreateClass(node.TypeConstructorContext, node.IsAbstract, node.IsConst,
            node.Name, GetGenericParameters(node), node.Supertypes);

    public static partial OrdinaryTypeConstructor StructDefinition_TypeConstructor(IStructDefinitionNode node)
        => BareTypeConstructor.CreateStruct(node.TypeConstructorContext, node.IsConst, node.Name,
            GetGenericParameters(node), node.Supertypes);

    public static partial OrdinaryTypeConstructor TraitDefinition_TypeConstructor(ITraitDefinitionNode node)
        => BareTypeConstructor.CreateTrait(node.TypeConstructorContext, node.IsConst, node.Name, GetGenericParameters(node), node.Supertypes);

    private static IFixedList<TypeConstructorParameter> GetGenericParameters(ITypeDefinitionNode node)
        => node.GenericParameters.Select(p => p.Parameter).ToFixedList();
    #endregion

    #region Type Definition Parts
    public static partial SelfTypeConstructor ImplicitSelfDefinition_TypeConstructor(IImplicitSelfDefinitionNode node)
        => node.TypeConstructorContext.SelfTypeConstructor;
    #endregion

    #region Member Definitions
    public static partial OrdinaryAssociatedTypeConstructor AssociatedTypeDefinition_TypeConstructor(IAssociatedTypeDefinitionNode node)
        => new(node.TypeConstructorContext, node.Name);
    #endregion
}
