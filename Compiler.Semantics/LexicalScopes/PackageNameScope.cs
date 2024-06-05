using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.Declared;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public sealed class PackageNameScope
{
    /// <summary>
    /// The global scope that spans all packages (current and referenced).
    /// </summary>
    /// <remarks>This is the root scope used for using directives.</remarks>
    public NamespaceScope UsingGlobalScope { get; }

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

    private readonly FixedDictionary<TypeName, ITypeDeclarationNode> primitives;

    internal PackageNameScope(
        IEnumerable<IPackageFacetNode> packageFacets,
        IEnumerable<IPackageFacetDeclarationNode> referencedFacets,
        IFixedSet<ITypeDeclarationNode> primitivesDeclarations)
    {
        var packageGlobalNamespaces = packageFacets.Select(f => f.GlobalNamespace).ToFixedSet();
        var referencedGlobalNamespaces = referencedFacets.Select(f => f.GlobalNamespace).ToFixedSet();

        UsingGlobalScope = new NamespaceScope(this, packageGlobalNamespaces.Concat(referencedGlobalNamespaces));

        // That parent scope is like the UsingGlobalScope, but includes only referenced packages
        // since the current package will already be searched by the PackageGlobalScope.
        var parent = new NamespaceScope(this, referencedGlobalNamespaces);
        PackageGlobalScope = new NamespaceScope(parent, packageGlobalNamespaces);

        packageGlobalScopes = packageGlobalNamespaces.Concat(referencedGlobalNamespaces)
            .GroupBy(ns => ns.Package.Name)
            .ToFixedDictionary(g => g.Key, g => new NamespaceScope(this, g));

        primitives = primitivesDeclarations.ToFixedDictionary(p => p.Name);
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
    public ITypeDeclarationNode? Lookup(DataType type)
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
        => primitives[declaredType.Name];

    private ITypeDeclarationNode Lookup(SimpleType declaredType)
        => primitives[declaredType.Name];

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
            UnknownAntetype _ => null,
            EmptyAntetype _ => null,
            FunctionAntetype _ => null,
            OptionalAntetype _ => throw new NotImplementedException(),
            NominalAntetype t => Lookup(t.Declared),
            AnyAntetype t => Lookup(t),
            SimpleAntetype t => Lookup(t),
            ConstValueAntetype _ => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    private ITypeDeclarationNode? Lookup(IDeclaredAntetype antetype)
        => antetype switch
        {
            EmptyAntetype _ => null,
            AnyAntetype t => Lookup(t),
            SimpleAntetype t => Lookup(t),
            GenericParameterAntetype t => Lookup(t),
            IUserDeclaredAntetype t => Lookup(t),
            _ => throw ExhaustiveMatch.Failed(antetype),
        };

    private ITypeDeclarationNode Lookup(IUserDeclaredAntetype antetype)
    {
        // TODO is there a problem with types using package names and this using package aliases?
        var globalNamespace = GlobalScopeForPackage(antetype.ContainingPackage);
        var ns = globalNamespace;
        foreach (var name in antetype.ContainingNamespace.Segments)
            ns = ns.GetChildNamespaceScope(name) ?? throw new UnreachableException("Type namespace must exist");

        return ns.Lookup(antetype.Name).OfType<ITypeDeclarationNode>().Single();
    }

    private ITypeDeclarationNode Lookup(SimpleAntetype antetype)
        => primitives[antetype.Name];

    private ITypeDeclarationNode Lookup(AnyAntetype antetype)
        => primitives[antetype.Name];

    public ITypeDeclarationNode Lookup(GenericParameterAntetype antetype)
    {
        var declaringTypeNode = (IUserTypeDeclarationNode)Lookup(antetype.DeclaringAntetype);
        return declaringTypeNode.GenericParameters.Single(p => p.Name == antetype.Name);
    }
    #endregion
}
