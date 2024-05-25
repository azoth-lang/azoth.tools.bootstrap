using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

/// <summary>
/// A scope that searches within namespaces.
/// </summary>
public abstract class NamespaceSearchScope : LexicalScope
{
    private protected NamespaceSearchScope() { }

    public abstract NamespaceScope? CreateChildNamespaceScope(IdentifierName namespaceName);
}
