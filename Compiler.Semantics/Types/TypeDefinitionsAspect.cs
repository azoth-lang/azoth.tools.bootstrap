using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class TypeDefinitionsAspect
{
    public static partial SelfType TypeDefinition_SelfType(ITypeDefinitionNode node)
        => new SelfType(node.DeclaredType);

    public static partial OrdinaryDeclaredType ClassDefinition_DeclaredType(IClassDefinitionNode node)
    {
        // TODO use ContainingDeclaredType in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);

        return OrdinaryDeclaredType.CreateClass(node.Package.Name, containingNamespaceName,
            node.IsAbstract, node.IsConst, node.Name,
            GetGenericParameters(node), node.Supertypes);
    }

    public static partial void ClassDefinition_Contribute_Diagnostics(IClassDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        CheckBaseTypeMustBeAClass(node, diagnostics);

        CheckBaseTypeMustMaintainIndependence(node, diagnostics);
    }

    private static void CheckBaseTypeMustBeAClass(IClassDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.BaseTypeName?.ReferencedSymbol is not null and not OrdinaryTypeSymbol { Kind: TypeKind.Class })
            diagnostics.Add(OtherSemanticError.BaseTypeMustBeClass(node.File, node.Name, node.BaseTypeName.Syntax));
    }

    private static void CheckBaseTypeMustMaintainIndependence(IClassDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var declaresType = node.Symbol.DeclaresType;
        // TODO nested classes and traits need to be checked if nested inside of generic types?
        if (!declaresType.HasIndependentGenericParameters) return;

        if (node.BaseTypeName is not null and var typeName
            && (!typeName.NamedBareType?.SupertypeMaintainsIndependence(exact: true) ?? false))
            diagnostics.Add(TypeError.SupertypeMustMaintainIndependence(node.File, typeName.Syntax));
    }

    public static partial OrdinaryDeclaredType StructDefinition_DeclaredType(IStructDefinitionNode node)
    {
        // TODO use ContainingDeclaredType in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return OrdinaryDeclaredType.CreateStruct(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name,
            GetGenericParameters(node), node.Supertypes);
    }

    public static partial OrdinaryDeclaredType TraitDefinition_DeclaredType(ITraitDefinitionNode node)
    {
        // TODO use ContainingDeclaredType in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return OrdinaryDeclaredType.CreateTrait(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name,
            GetGenericParameters(node), node.Supertypes);
    }

    private static NamespaceName GetContainingNamespaceName(ITypeMemberDefinitionNode node)
    {
        // TODO correctly deal with containing namespace
        var containingSymbol = node.ContainingSymbol!; // Since it is a type or namespace, it will have a symbol
        while (containingSymbol is not NamespaceSymbol) containingSymbol = containingSymbol.ContainingSymbol!;
        var containingNamespaceName = ((NamespaceSymbol)containingSymbol).NamespaceName;
        return containingNamespaceName;
    }

    private static IFixedList<GenericParameter> GetGenericParameters(ITypeDefinitionNode node)
        => node.GenericParameters.Select(p => p.Parameter).ToFixedList();

    public static partial GenericParameter GenericParameter_Parameter(IGenericParameterNode node)
        => new GenericParameter(node.Constraint.Constraint, node.Name, node.Independence, node.Variance);

    public static partial GenericParameterType GenericParameter_DeclaredType(IGenericParameterNode node)
        => node.ContainingDeclaredType.GenericParameterTypes.Single(t => t.Parameter == node.Parameter);

    public static partial IFixedSet<BareNonVariableType> TypeDefinition_Supertypes(ITypeDefinitionNode node)
    {
        // Note: Supertypes is a circular attribute that both declared types and symbols depend on.
        // While there are many ways to write this that will give the correct answer, care should be
        // taken to avoid causing unnecessary effective cycles. That way attributes can be cached
        // and performance can be improved.

        // Avoid loading the declared type unless necessary because it is a cycle and accessing it
        // prevents caching.
        OrdinaryDeclaredType? declaredType = null;
        return Build()
               // Exclude any cycles that exist in the supertypes by excluding this type
               .Where(t =>
                   // Try to avoid loading the declared type by checking names first
                   t.TypeConstructor.Name != node.Name
                   // But the real check is on the declared type
                   || !t.TypeConstructor.Equals(declaredType ??= node.DeclaredType))
               // Everything has `Any` as a supertype (added after filter to avoid loading declared type)
               .Append(BareNonVariableType.Any)
               .ToFixedSet();

        IEnumerable<BareNonVariableType> Build()
        {
            // Handled by supertype because that is the only syntax we have to apply the compiler
            // errors to. (Could possibly use type arguments in the future.)
            foreach (var supertypeName in node.AllSupertypeNames)
            {
                if (supertypeName.NamedBareType is not BareNonVariableType { TypeConstructor.CanBeSupertype: true } bareSupertype)
                    // A diagnostic will be generated elsewhere for this case
                    continue;

                // First, include the direct supertype
                yield return bareSupertype;

                var referencedDeclaration = supertypeName.ReferencedDeclaration;
                if (referencedDeclaration is null)
                    // A diagnostic will be generated elsewhere for this case
                    continue;

                // Recurse directly on the supertypes property rather than using the supertypes of
                // the bare type. That way the circular attribute can handle cycles.
                foreach (var supertype in referencedDeclaration.Supertypes)
                {
                    // Since this is our supertype's supertype, we need to replace any type
                    // parameters with those given to the supertype.
                    yield return bareSupertype.ReplaceTypeParametersIn(supertype);
                }
            }
        }
    }

    public static partial void TypeDefinition_Contribute_Diagnostics(ITypeDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        CheckSupertypesForCycle(node, diagnostics);

        CheckTypeArgumentsAreConstructable(node, diagnostics);

        CheckSupertypesMustBeClassOrTrait(node, diagnostics);

        // TODO check that there aren't duplicate supertypes? (including base type)

        CheckAllSupertypesAreOutputSafe(node, diagnostics);
        CheckSupertypesMaintainIndependence(node, diagnostics);
    }

    private static void CheckSupertypesForCycle(ITypeDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var declaredType = node.DeclaredType;
        foreach (var supertypeName in node.AllSupertypeNames)
            if (supertypeName.InheritsFrom(declaredType))
                diagnostics.Add(OtherSemanticError.CircularDefinitionInSupertype(node.File, supertypeName.Syntax));
    }

    private static bool InheritsFrom(this IStandardTypeNameNode node, OrdinaryDeclaredType type)
        => node.ReferencedDeclaration?.Supertypes.Any(t => t.TypeConstructor.Equals(type)) ?? false;

    private static void CheckTypeArgumentsAreConstructable(ITypeDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        foreach (IStandardTypeNameNode supertypeName in node.SupertypeNames)
            ExpressionTypesAspect.CheckTypeArgumentsAreConstructable(supertypeName, diagnostics);
    }

    private static void CheckSupertypesMustBeClassOrTrait(ITypeDefinitionNode typeNode, DiagnosticCollectionBuilder diagnostics)
    {
        foreach (var node in typeNode.SupertypeNames)
            // Null symbol will report a separate name binding error
            if (node.ReferencedSymbol is OrdinaryTypeSymbol { Kind: TypeKind.Struct })
                diagnostics.Add(OtherSemanticError.SupertypeMustBeClassOrTrait(node.File, typeNode.Name, node.Syntax));
    }

    private static void CheckAllSupertypesAreOutputSafe(ITypeDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var declaresType = node.Symbol.DeclaresType;
        // TODO nested classes and traits need to be checked if nested inside of generic types
        if (!declaresType.IsGeneric) return;
        var nonwritableSelf = declaresType.IsDeclaredConst ? true : (bool?)null;
        foreach (var typeName in node.AllSupertypeNames)
        {
            var type = typeName.NamedBareType;
            if (type is not null && !type.IsSupertypeOutputSafe(nonwritableSelf))
                diagnostics.Add(TypeError.SupertypeMustBeOutputSafe(node.File, typeName.Syntax));
        }
    }

    private static void CheckSupertypesMaintainIndependence(
        ITypeDefinitionNode typeDefinition,
        DiagnosticCollectionBuilder diagnostics)
    {
        var declaresType = typeDefinition.Symbol.DeclaresType;
        // TODO nested classes and traits need to be checked if nested inside of generic types?
        if (!declaresType.HasIndependentGenericParameters) return;

        foreach (var typeName in typeDefinition.SupertypeNames)
            if (!typeName.NamedBareType?.SupertypeMaintainsIndependence(exact: false) ?? false)
                diagnostics.Add(TypeError.SupertypeMustMaintainIndependence(typeDefinition.File, typeName.Syntax));
    }
}
