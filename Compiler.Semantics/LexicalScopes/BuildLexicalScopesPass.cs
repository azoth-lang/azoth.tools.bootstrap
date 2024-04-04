using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public sealed class BuildLexicalScopesPass : IPass<Void, Concrete.Package, Scoped.Package>
{
    public static Scoped.Package Run(Void context, Concrete.Package from)
    {
        var pass = new BuildLexicalScopesPass(context);
        return pass.Run(from);
    }

    private BuildLexicalScopesPass(Void context) { }

    public Scoped.Package Run(Concrete.Package from) => throw new System.NotImplementedException();
}
