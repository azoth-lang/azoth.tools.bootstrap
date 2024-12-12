using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public sealed class PackageNameScope
{
    /// <summary>
    /// The global scope that spans all packages (current and referenced).
    /// </summary>
    /// <remarks>This is the root scope used for import directives.</remarks>
    public NamespaceScope ImportGlobalScope { get; }

    /// <summary>
    /// The global scope for this package.
    /// </summary>
    /// <remarks>This scope first searches the current scope, but then its parent searches the global
    /// scope of all referenced packages.</remarks>
    public NamespaceScope PackageGlobalScope { get; }

    /// <summary>
    /// The global scopes for each package.
    /// </summary>
    private readonly FixedDictionary<IdentifierName, NamespaceScope> packageGlobalScopes;

    private readonly FixedDictionary<TypeName, ITypeDeclarationNode> builtIns;

    internal PackageNameScope(
        IEnumerable<IPackageFacetNode> packageFacets,
        IEnumerable<IPackageFacetDeclarationNode> referencedFacets,
        IFixedSet<ITypeDeclarationNode> builtInDeclarations)
    {
        var packageGlobalNamespaces = packageFacets.Select(f => f.GlobalNamespace).ToFixedSet();
        var referencedGlobalNamespaces = referencedFacets.Select(f => f.GlobalNamespace).ToFixedSet();

        ImportGlobalScope = new NamespaceScope(this, packageGlobalNamespaces.Concat(referencedGlobalNamespaces));

        // That parent scope is like the ImportGlobalScope, but includes only referenced packages
        // since the current package will already be searched by the PackageGlobalScope.
        var parent = new NamespaceScope(this, referencedGlobalNamespaces);
        PackageGlobalScope = new NamespaceScope(parent, packageGlobalNamespaces);

        packageGlobalScopes = packageGlobalNamespaces.Concat(referencedGlobalNamespaces)
            .GroupBy(ns => ns.Package.Name)
            .ToFixedDictionary(g => g.Key, g => new NamespaceScope(this, g));

        builtIns = builtInDeclarations.ToFixedDictionary(p => p.Name);
    }

    /// <summary>
    /// Get the global scope for a referenced package.
    /// </summary>
    /// <remarks>This provides the root for names that are package qualified. Currently, this
    /// includes the current package.</remarks>
    public NamespaceScope GlobalScopeForPackage(IdentifierName packageName)
    {
        if (packageGlobalScopes.TryGetValue(packageName, out var scope))
            return scope;
        throw new InvalidOperationException($"Package '{packageName}' is not referenced.");
    }

    #region Loopkup(Type)
    public ITypeDeclarationNode? Lookup(IMaybeType type)
        => type switch
        {
            UnknownType _ => null,
            NeverType _ => null,
            VoidType _ => null,
            FunctionType _ => null,
            OptionalType _ => throw new NotImplementedException(),
            CapabilityType t => Lookup(t.PlainType.TypeConstructor),
            GenericParameterType t => Lookup(t),
            SelfViewpointType t => Lookup(t.Referent),
            CapabilitySetSelfType t => null,
            _ => throw ExhaustiveMatch.Failed(type),
        };

    public ITypeDeclarationNode Lookup(GenericParameterType type)
        => throw new NotImplementedException();
    #endregion

    #region Lookup(*PlainType)
    public ITypeDeclarationNode? Lookup(IMaybePlainType plainType)
        => plainType switch
        {
            UnknownPlainType _ => null,
            VoidPlainType _ => null,
            NeverPlainType _ => null,
            FunctionPlainType _ => null,
            OptionalPlainType _ => throw new NotImplementedException(),
            GenericParameterPlainType t => Lookup(t),
            OrdinaryAssociatedPlainType _ => null,
            SelfPlainType _ => null,
            ConstructedPlainType t => Lookup(t.TypeConstructor),
            _ => throw ExhaustiveMatch.Failed(plainType),
        };

    public ITypeDeclarationNode Lookup(GenericParameterPlainType plainType)
    {
        var declaringTypeNode = (IUserTypeDeclarationNode)Lookup(plainType.DeclaringTypeConstructor);
        return declaringTypeNode.GenericParameters.Single(p => p.Name == plainType.Name);
    }
    #endregion

    #region Lookup(*TypeConstructor)
    public ITypeDeclarationNode? Lookup(TypeConstructor? typeConstructor)
        => typeConstructor switch
        {
            null => null,
            SimpleTypeConstructor t => Lookup(t),
            // TODO There are no declarations for const value type, but perhaps there should be?
            LiteralTypeConstructor _ => null,
            AnyTypeConstructor t => Lookup(t),
            OrdinaryTypeConstructor t => Lookup(t),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public ITypeDeclarationNode Lookup(OrdinaryTypeConstructor typeConstructor)
    {
        // TODO is there a problem with types using package names and this using package aliases?
        // TODO handle nested types that have another type as their context
        var context = (NamespaceContext)typeConstructor.Context;
        var globalNamespace = GlobalScopeForPackage(context.Package);
        var ns = globalNamespace;
        foreach (var name in context.Namespace.Segments)
            ns = ns.GetChildNamespaceScope(name) ?? throw new UnreachableException("Type namespace must exist");

        return ns.Lookup(typeConstructor.Name).OfType<ITypeDeclarationNode>().Single();
    }

    public ITypeDeclarationNode Lookup(SimpleTypeConstructor typeConstructor)
        => builtIns[typeConstructor.Name];

    public ITypeDeclarationNode Lookup(AnyTypeConstructor typeConstructor)
        => builtIns[typeConstructor.Name];
    #endregion
}
