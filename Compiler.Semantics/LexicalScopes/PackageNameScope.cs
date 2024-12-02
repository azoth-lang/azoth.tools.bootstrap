using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
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

    #region Loopkup(DataType)
    public ITypeDeclarationNode? Lookup(IMaybeExpressionType type)
        => type switch
        {
            UnknownType _ => null,
            EmptyType _ => null,
            FunctionType _ => null,
            OptionalType _ => throw new NotImplementedException(),
            ConstValueType _ => throw new NotImplementedException(),
            CapabilityType t => Lookup(t.DeclaredType),
            GenericParameterType t => Lookup(t),
            ViewpointType t => Lookup(t.Referent),
            _ => throw ExhaustiveMatch.Failed(type),
        };

    public ITypeDeclarationNode Lookup(GenericParameterType type)
        => throw new NotImplementedException();

    private ITypeDeclarationNode Lookup(DeclaredType declaredType)
        => declaredType switch
        {
            AnyType t => Lookup(t),
            ObjectType t => Lookup((IDeclaredUserType)t),
            SimpleType t => Lookup(t),
            StructType t => Lookup((IDeclaredUserType)t),
            _ => throw ExhaustiveMatch.Failed(declaredType),
        };

    private ITypeDeclarationNode Lookup(AnyType declaredType)
        => builtIns[declaredType.Name];

    private ITypeDeclarationNode Lookup(SimpleType declaredType)
        => builtIns[declaredType.Name];

    private ITypeDeclarationNode Lookup(IDeclaredUserType declaredType)
    {
        // TODO is there a problem with types using package names and this using package aliases?
        var globalNamespace = GlobalScopeForPackage(declaredType.ContainingPackage);
        var ns = globalNamespace;
        foreach (var name in declaredType.ContainingNamespace.Segments)
            ns = ns.GetChildNamespaceScope(name) ?? throw new UnreachableException("Type namespace must exist");

        return ns.Lookup(declaredType.Name).OfType<ITypeDeclarationNode>().Single();
    }
    #endregion

    #region Lookup(IMaybeExpressionAntetype)
    public ITypeDeclarationNode? Lookup(IMaybeExpressionAntetype antetype)
        => antetype switch
        {
            UnknownPlainType _ => null,
            EmptyPlainType _ => null,
            FunctionPlainType _ => null,
            OptionalPlainType _ => throw new NotImplementedException(),
            AnyAntetype t => Lookup(t),
            GenericParameterPlainType t => Lookup(t),
            SelfPlainType _ => null,
            OrdinaryNamedPlainType t => Lookup(t.TypeConstructor),
            SimpleTypeConstructor t => Lookup(t),
            // TODO There are no declarations for const value type, but perhaps there should be?
            LiteralTypeConstructor _ => null,
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    private ITypeDeclarationNode? Lookup(ITypeConstructor antetype)
        => antetype switch
        {
            SimpleTypeConstructor t => Lookup(t),
            OrdinaryTypeConstructor t => Lookup(t),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    private ITypeDeclarationNode Lookup(OrdinaryTypeConstructor antetype)
    {
        // TODO is there a problem with types using package names and this using package aliases?
        var globalNamespace = GlobalScopeForPackage(antetype.ContainingPackage);
        var ns = globalNamespace;
        foreach (var name in antetype.ContainingNamespace.Segments)
            ns = ns.GetChildNamespaceScope(name) ?? throw new UnreachableException("Type namespace must exist");

        return ns.Lookup(antetype.Name).OfType<ITypeDeclarationNode>().Single();
    }

    private ITypeDeclarationNode Lookup(SimpleTypeConstructor typeConstructor)
        => builtIns[typeConstructor.Name];

    private ITypeDeclarationNode Lookup(AnyAntetype antetype)
        => builtIns[antetype.Name];

    public ITypeDeclarationNode Lookup(GenericParameterPlainType plainType)
    {
        var declaringTypeNode = (IUserTypeDeclarationNode)Lookup(plainType.DeclaringAntetype);
        return declaringTypeNode.GenericParameters.Single(p => p.Name == plainType.Name);
    }
    #endregion
}
