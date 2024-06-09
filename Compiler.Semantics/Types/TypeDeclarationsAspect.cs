using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class TypeDeclarationsAspect
{
    public static ObjectType ClassDefinition_DeclaredType(IClassDefinitionNode node)
    {
        // TODO use ContainingDeclaredType in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);

        return ObjectType.CreateClass(node.Package.Name, containingNamespaceName,
            node.IsAbstract, node.IsConst, node.Name,
            GetGenericParameters(node), LazySupertypes(node));
    }

    public static void ClassDeclaration_ContributeDiagnostics(IClassDefinitionNode node, Diagnostics diagnostics)
    {
        CheckBaseTypeMustBeAClass(node, diagnostics);

        CheckBaseTypeMustMaintainIndependence(node, diagnostics);
    }

    private static void CheckBaseTypeMustBeAClass(IClassDefinitionNode node, Diagnostics diagnostics)
    {
        if (node.BaseTypeName?.ReferencedSymbol is not null and not UserTypeSymbol { DeclaresType.IsClass: true })
            diagnostics.Add(OtherSemanticError.BaseTypeMustBeClass(node.File, node.Name, node.BaseTypeName.Syntax));
    }

    private static void CheckBaseTypeMustMaintainIndependence(IClassDefinitionNode node, Diagnostics diagnostics)
    {
        var declaresType = node.Symbol.DeclaresType;
        // TODO nested classes and traits need to be checked if nested inside of generic types?
        if (!declaresType.HasIndependentGenericParameters) return;

        if (node.BaseTypeName is not null and var typeName
            && (!typeName.BareType?.SupertypeMaintainsIndependence(exact: true) ?? false))
            diagnostics.Add(TypeError.SupertypeMustMaintainIndependence(node.File, typeName.Syntax));
    }

    public static StructType StructDefinition_DeclaredType(IStructDefinitionNode node)
    {
        // TODO use ContainingDeclaredType in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return StructType.Create(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name,
            GetGenericParameters(node), LazySupertypes(node));
    }

    public static ObjectType TraitDefinition_DeclaredType(ITraitDefinitionNode node)
    {
        // TODO use ContainingDeclaredType in case this is a nested type
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return ObjectType.CreateTrait(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name,
            GetGenericParameters(node), LazySupertypes(node));
    }

    private static NamespaceName GetContainingNamespaceName(ITypeMemberDefinitionNode node)
    {
        // TODO correctly deal with containing namespace
        var containingSymbol = node.ContainingSymbol;
        while (containingSymbol is not NamespaceSymbol) containingSymbol = containingSymbol.ContainingSymbol!;
        var containingNamespaceName = ((NamespaceSymbol)containingSymbol).NamespaceName;
        return containingNamespaceName;
    }

    private static IFixedList<GenericParameter> GetGenericParameters(ITypeDefinitionNode node)
        => node.GenericParameters.Select(p => p.Parameter).ToFixedList();

    private static Lazy<IFixedSet<BareReferenceType>> LazySupertypes(ITypeDefinitionNode node)
        // Use PublicationOnly so that initialization cycles are detected and thrown by the attributes
        => new(() => node.Supertypes.Value, LazyThreadSafetyMode.PublicationOnly);

    public static GenericParameter GenericParameter_Parameter(IGenericParameterNode node)
        => new GenericParameter(node.Constraint.Constraint, node.Name, node.Independence, node.Variance);

    public static GenericParameterType GenericParameter_DeclaredType(IGenericParameterNode node)
        => node.ContainingDeclaredType.GenericParameterTypes.Single(t => t.Parameter == node.Parameter);

    public static CompilerResult<IFixedSet<BareReferenceType>> TypeDeclaration_Supertypes(ITypeDefinitionNode node)
    {
        // Avoid creating the diagnostic list unless needed since typically there are no diagnostics
        List<Diagnostic>? diagnostics = null;
        return new(Build().ToFixedSet(), diagnostics?.ToFixedSet() ?? FixedSet.Empty<Diagnostic>());

        IEnumerable<BareReferenceType> Build()
        {
            // Everything has `Any` as a supertype
            yield return BareType.Any;

            // Handled by supertype because that is the only syntax we have to apply the compiler
            // errors to. (Could possibly use type arguments in the future.)
            foreach (var supertypeName in node.AllSupertypeNames)
            {
                if (supertypeName.BareType is not BareReferenceType bareSupertype)
                    // A diagnostic will be generated elsewhere for this case
                    continue;

                IFixedSet<BareReferenceType> inheritedTypes;
                try
                {
                    inheritedTypes = bareSupertype.Supertypes;
                }
                catch (AttributeCycleException)
                {
                    AddDiagnostic(OtherSemanticError.CircularDefinitionInSupertype(node.File, supertypeName.Syntax));
                    continue;
                }

                // Note: `yield return` must be done outside of the try block (C# language rule)
                foreach (var inheritedType in inheritedTypes)
                    yield return inheritedType;
                yield return bareSupertype;
            }
        }

        void AddDiagnostic(Diagnostic diagnostic)
        {
            diagnostics ??= new List<Diagnostic>();
            diagnostics.Add(diagnostic);
        }
    }

    public static void TypeDeclaration_ContributeDiagnostics(ITypeDefinitionNode node, Diagnostics diagnostics)
    {
        // Record diagnostics created while computing supertypes
        diagnostics.Add(node.Supertypes.Diagnostics);

        CheckTypeArgumentsAreConstructable(node, diagnostics);

        CheckSupertypesMustBeClassOrTrait(node, diagnostics);

        // TODO check that there aren't duplicate supertypes? (including base type)

        CheckAllSupertypesAreOutputSafe(node, diagnostics);
        CheckSupertypesMaintainIndependence(node, diagnostics);
    }

    private static void CheckTypeArgumentsAreConstructable(ITypeDefinitionNode node, Diagnostics diagnostics)
    {
        foreach (IStandardTypeNameNode supertypeName in node.SupertypeNames)
            ExpressionTypesAspect.CheckTypeArgumentsAreConstructable(supertypeName, diagnostics);
    }

    private static void CheckSupertypesMustBeClassOrTrait(ITypeDefinitionNode typeNode, Diagnostics diagnostics)
    {
        foreach (var node in typeNode.SupertypeNames)
            // Null symbol will report a separate name binding error
            if (node.ReferencedSymbol is not null and not UserTypeSymbol { DeclaresType: ObjectType })
                diagnostics.Add(OtherSemanticError.SupertypeMustBeClassOrTrait(node.File, typeNode.Name, node.Syntax));
    }

    private static void CheckAllSupertypesAreOutputSafe(ITypeDefinitionNode node, Diagnostics diagnostics)
    {
        var declaresType = node.Symbol.DeclaresType;
        // TODO nested classes and traits need to be checked if nested inside of generic types
        if (!declaresType.IsGeneric) return;
        var nonwritableSelf = declaresType.IsDeclaredConst ? true : (bool?)null;
        foreach (var typeName in node.AllSupertypeNames)
        {
            var type = typeName.BareType;
            if (type is not null && !type.IsSupertypeOutputSafe(nonwritableSelf))
                diagnostics.Add(TypeError.SupertypeMustBeOutputSafe(node.File, typeName.Syntax));
        }
    }

    private static void CheckSupertypesMaintainIndependence(
        ITypeDefinitionNode typeDefinition,
        Diagnostics diagnostics)
    {
        var declaresType = typeDefinition.Symbol.DeclaresType;
        // TODO nested classes and traits need to be checked if nested inside of generic types?
        if (!declaresType.HasIndependentGenericParameters) return;

        foreach (var typeName in typeDefinition.SupertypeNames)
            if (!typeName.BareType?.SupertypeMaintainsIndependence(exact: false) ?? false)
                diagnostics.Add(TypeError.SupertypeMustMaintainIndependence(typeDefinition.File, typeName.Syntax));
    }
}
