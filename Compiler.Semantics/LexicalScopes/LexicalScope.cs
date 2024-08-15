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

    public abstract IEnumerable<IDeclarationNode> Lookup(StandardName name);
}
