using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

/// <summary>
/// A lexical scope that supports lookup of symbol nodes by name.
/// </summary>
public abstract class LexicalScope
{
    public abstract PackageNameScope PackageNames { get; }

    private protected LexicalScope() { }

    /// <summary>
    /// Lookup an <see cref="OrdinaryName"/> in the scope and find any matching declarations.
    /// </summary>
    public IEnumerable<IDeclarationNode> Lookup(OrdinaryName name)
        => Lookup<IDeclarationNode>(name);

    /// <summary>
    /// Lookup an <see cref="OrdinaryName"/> in the scope and find matching declarations of the type
    /// <typeparamref name="TDeclaration"/>.
    /// </summary>
    public abstract IEnumerable<TDeclaration> Lookup<TDeclaration>(OrdinaryName name)
        where TDeclaration : IDeclarationNode;

    public virtual ITypeDeclarationNode? Lookup(BuiltInTypeName name)
        => PackageNames.Lookup(name);
}
