using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Framework;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public sealed partial class DeclarationLexicalScopesBuilder : ITransformPass<From.Package, Void, To.Package, Void>
{
    public static To.Package Run(From.Package from)
    {
        var pass = new DeclarationLexicalScopesBuilder();
        pass.StartRun();
        var to = pass.Transform(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, Void, To.Package, Void>.Run(From.Package from, Void context)
        => (Run(from), default);

    partial void StartRun();

    partial void EndRun(To.Package package);

    private partial To.Package Transform(From.Package from);
}
