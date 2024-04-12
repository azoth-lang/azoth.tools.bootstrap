using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.LexicalScopes;

/// <summary>
/// A lexical scope induced by a declaration (e.g. a namespace or class).
/// </summary>
/// <remarks>
/// <para>This kind of lexical scope does not apply to method and function bodies. They are a
/// distinct case. This rather us used for namespace and type declarations that could hold other
/// namespaces, types, functions, methods, fields, etc.</para>
/// </remarks>
public abstract class DeclarationLexicalScope
{
    /// <summary>
    /// The <see cref="PackageReferenceScope"/> that is the ancestor of this scope.
    /// </summary>
    /// <remarks>This is the root for the tree of scopes.</remarks>
    public abstract PackageReferenceScope RootScope { get; }

    /// <summary>
    /// Resolve the given name in this scope in the referenced packages.
    /// </summary>
    public IEnumerable<Symbol> ResolveInReferences(TypeName name)
    {
        return name switch
        {
            SpecialTypeName specialTypeName => Resolve(specialTypeName).Yield(),
            StandardName standardName => ResolveInReferences(standardName),
            _ => throw ExhaustiveMatch.Failed(name)
        };
    }

    /// <summary>
    /// Resolve the given name in this scope in the referenced packages.
    /// </summary>
    public PrimitiveTypeSymbol Resolve(SpecialTypeName name)
        => Primitive.SymbolTree.LookupSymbol(name);

    /// <summary>
    /// Resolve the given name in this scope in the referenced packages.
    /// </summary>
    public abstract IEnumerable<Symbol> ResolveInReferences(StandardName name);
}
