using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;
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

    private readonly FixedDictionary<UnqualifiedName, ITypeDeclarationNode> builtIns;

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
            .GroupBy(ns => ns.PackageSymbol.Name)
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

    #region Loopkup(BuiltInTypeName)
    public ITypeDeclarationNode? Lookup(BuiltInTypeName name) => builtIns.GetValueOrDefault(name);
    #endregion

    #region Lookup(*PlainType)
    public ITypeDeclarationNode? Lookup(IMaybePlainType plainType)
        => plainType switch
        {
            UnknownPlainType _ => null,
            VoidPlainType _ => null,
            NeverPlainType _ => null,
            FunctionPlainType _ => null,
            RefPlainType _ => null,
            OptionalPlainType _ => throw new NotImplementedException(),
            GenericParameterPlainType t => Lookup(t),
            BarePlainType t => Lookup(t.TypeConstructor),
            _ => throw ExhaustiveMatch.Failed(plainType),
        };

    public ITypeDeclarationNode Lookup(GenericParameterPlainType plainType)
    {
        var declaringTypeNode = (IOrdinaryTypeDeclarationNode)Lookup(plainType.DeclaringTypeConstructor);
        return declaringTypeNode.GenericParameters.Single(p => p.Name == plainType.Name);
    }
    #endregion

    #region Lookup(*TypeConstructor)
    public ITypeDeclarationNode? Lookup(BareTypeConstructor? typeConstructor)
        => typeConstructor switch
        {
            null => null,
            SimpleTypeConstructor t => Lookup(t),
            // TODO There are no declarations for literal value type, but perhaps there should be?
            LiteralTypeConstructor _ => null,
            AnyTypeConstructor t => Lookup(t),
            OrdinaryTypeConstructor t => Lookup(t),
            AssociatedTypeConstructor t => Lookup(t),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public ITypeDeclarationNode Lookup(OrdinaryTypeConstructor typeConstructor)
    {
        return typeConstructor.Context switch
        {
            NamespaceContext c => Lookup(c, typeConstructor.Name),
            BareTypeConstructor c => Lookup(Lookup(c)!, typeConstructor),
            BuiltInContext _ => throw new UnreachableException($"`{nameof(OrdinaryTypeConstructor)}` cannot have `{nameof(BuiltInContext)}`."),
            _ => throw ExhaustiveMatch.Failed(typeConstructor.Context),
        };
    }

    private ITypeDeclarationNode Lookup(NamespaceContext context, OrdinaryName name)
    {
        // TODO is there a problem with types using package names and this using package aliases?
        var globalNamespace = GlobalScopeForPackage(context.Package);
        var ns = globalNamespace;
        foreach (var nsName in context.Namespace.Segments)
            ns = ns.GetChildNamespaceScope(nsName) ?? throw new UnreachableException("Type namespace must exist");

        return ns.Lookup(name).OfType<ITypeDeclarationNode>().Single();
    }

    private static ITypeDeclarationNode Lookup(ITypeDeclarationNode context, OrdinaryTypeConstructor typeConstructor)
        => context.TypeMembersNamed(typeConstructor.Name)
                  .Single(d => d.TypeConstructor.Equals(typeConstructor));

    public ITypeDeclarationNode Lookup(AssociatedTypeConstructor typeConstructor)
        => typeConstructor switch
        {
            OrdinaryAssociatedTypeConstructor t => Lookup(t),
            SelfTypeConstructor t => Lookup(t),
            _ => throw ExhaustiveMatch.Failed(typeConstructor),
        };

    public ITypeDeclarationNode Lookup(OrdinaryAssociatedTypeConstructor typeConstructor)
    {
        // TODO remove need for `!`
        var context = Lookup(typeConstructor.Context)!;
        return context.AssociatedMembersNamed(typeConstructor.Name).OfType<ITypeDeclarationNode>().Single();
    }

    public ITypeDeclarationNode Lookup(SelfTypeConstructor typeConstructor)
    {
        // TODO remove need for cast
        var context = (INonVariableTypeDeclarationNode)Lookup(typeConstructor.Context)!;
        return context.ImplicitSelf;
    }

    public ITypeDeclarationNode Lookup(SimpleTypeConstructor typeConstructor)
        => builtIns[typeConstructor.Name];

    public ITypeDeclarationNode Lookup(AnyTypeConstructor typeConstructor)
        => builtIns[typeConstructor.Name];
    #endregion
}
