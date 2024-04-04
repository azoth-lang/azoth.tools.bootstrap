using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public sealed partial class BuildLexicalScopesPass : IPass<Void, Concrete.Package, Scoped.Package>
{
    public static Scoped.Package Run(Void context, Concrete.Package from)
    {
        var pass = new BuildLexicalScopesPass(context);
        return pass.Run(from);
    }

    public Scoped.Package Run(Concrete.Package from) => Transform(from);

    private partial Scoped.Package Transform(Concrete.Package from);

    private partial (NestedScope, FixedDictionary<NamespaceName, Namespace> namespaces, LexicalScope) Enter(
        IFixedList<Concrete.CompilationUnit> from,
        PackagesScope packagesScope,
        IFixedList<NonMemberSymbol> declarationSymbols);

    private partial Scoped.Declaration Transform(Concrete.Declaration from, LexicalScope containingScope);
}
