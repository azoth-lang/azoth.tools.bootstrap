using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    private readonly FixedDictionary<IdentifierName, NamespaceScope> referencedGlobalScopes;

    internal PackageNameScope(IEnumerable<IPackageFacetNode> packageFacets, IEnumerable<IPackageFacetDeclarationNode> referencedFacets)
    {
        var packageGlobalNamespaces = packageFacets.Select(f => f.GlobalNamespace).ToFixedSet();
        var referencedGlobalNamespaces = referencedFacets.Select(f => f.GlobalNamespace).ToFixedSet();

        UsingGlobalScope = new NamespaceScope(this, packageGlobalNamespaces.Concat(referencedGlobalNamespaces));

        // That parent scope is like the UsingGlobalScope, but includes only referenced packages
        // since the current package will already be searched by the PackageGlobalScope.
        var parent = new NamespaceScope(this, referencedGlobalNamespaces);
        PackageGlobalScope = new NamespaceScope(parent, packageGlobalNamespaces);

        referencedGlobalScopes = referencedGlobalNamespaces.GroupBy(ns => ns.Package.Name)
            .ToFixedDictionary(g => g.Key, g => new NamespaceScope(this, g));
    }

    /// <summary>
    /// Get the global scope for a referenced package.
    /// </summary>
    /// <remarks>This provides the root for names that are package qualified.</remarks>
    public NamespaceScope GlobalScopeForReferencedPackage(IdentifierName packageName)
    {
        if (referencedGlobalScopes.TryGetValue(packageName, out var scope))
            return scope;
        throw new InvalidOperationException($"Package '{packageName}' is not referenced.");
    }

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
        => throw new NotImplementedException();

    private ITypeDeclarationNode Lookup(SimpleType declaredType)
        => throw new NotImplementedException();

    private ITypeDeclarationNode Lookup(IDeclaredUserType declaredType)
    {
        // TODO is there a problem with types using package names and this using package aliases?
        var globalNamespace = GlobalScopeForReferencedPackage(declaredType.ContainingPackage);
        var ns = globalNamespace;
        foreach (var name in declaredType.ContainingNamespace.Segments)
            ns = ns.GetChildNamespaceScope(name) ?? throw new UnreachableException("Type namespace must exist");

        return ns.Lookup(declaredType.Name).OfType<ITypeDeclarationNode>().Single();
    }
}
