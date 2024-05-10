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
    public static ObjectType ClassDeclaration_DeclaredType(IClassDeclarationNode node)
    {
        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);

        return ObjectType.CreateClass(node.Package.Name, containingNamespaceName,
            node.IsAbstract, node.IsConst,
            node.Name, GetGenericParameters(node), LazySupertypes(node));
    }

    public static CompilerResult<IFixedSet<BareReferenceType>> ClassDeclaration_Supertypes(IClassDeclarationNode node)
        => BuildSupertypes(node, node.BaseTypeName.YieldValue().Concat(node.SupertypeNames));

    public static StructType StructDeclaration_DeclaredType(IStructDeclarationNode node)
    {
        var genericParameters = GetGenericParameters(node);

        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return StructType.Create(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name,
            genericParameters, LazySupertypes(node));
    }

    public static CompilerResult<IFixedSet<BareReferenceType>> StructDeclaration_Supertypes(IStructDeclarationNode node)
        => BuildSupertypes(node, node.SupertypeNames);

    public static ObjectType TraitDeclaration_DeclaredType(ITraitDeclarationNode node)
    {
        var genericParameters = GetGenericParameters(node);

        NamespaceName containingNamespaceName = GetContainingNamespaceName(node);
        return ObjectType.CreateTrait(node.Package.Name, containingNamespaceName,
            node.IsConst, node.Name,
            genericParameters, LazySupertypes(node));
    }

    public static CompilerResult<IFixedSet<BareReferenceType>> TraitDeclaration_Supertypes(ITraitDeclarationNode node)
        => BuildSupertypes(node, node.SupertypeNames);

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

    private static Lazy<IFixedSet<BareReferenceType>> LazySupertypes(ITypeDeclarationNode node)
        // Use PublicationOnly so that initialization cycles are detected and thrown by the attributes
        => new(() => node.Supertypes.Value, LazyThreadSafetyMode.PublicationOnly);

    private static CompilerResult<IFixedSet<BareReferenceType>> BuildSupertypes(
        ITypeDeclarationNode typeNode,
        IEnumerable<IStandardTypeNameNode> supertypeNames)
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
            foreach (var supertypeName in supertypeNames)
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
                    AddDiagnostic(OtherSemanticError.CircularDefinitionInSupertype(typeNode.File, supertypeName.Syntax));
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

    public static void TypeDeclaration_ContributeDiagnostics(ITypeDeclarationNode node, Diagnostics diagnostics)
    {
        diagnostics.Add(node.Supertypes.Diagnostics);

        diagnostics.Add(CheckTypeArgumentsAreConstructable(node));
    }

    private static IEnumerable<Diagnostic> CheckTypeArgumentsAreConstructable(ITypeDeclarationNode node)
    {
        foreach (IStandardTypeNameNode supertypeName in node.SupertypeNames)
        {
            var bareType = supertypeName.BareType;
            if (bareType is null)
                continue;

            foreach (GenericParameterArgument arg in bareType.GenericParameterArguments)
                if (!arg.IsConstructable())
                    yield return TypeError.CapabilityNotCompatibleWithConstraint(node.File, supertypeName.Syntax,
                        arg.Parameter, arg.Argument);
        }
    }

    public static GenericParameterType GenericParameter_DeclaredType(IGenericParameterNode node)
        => node.ContainingDeclaredType.GenericParameterTypes.Single(t => t.Parameter == node.Parameter);
}
