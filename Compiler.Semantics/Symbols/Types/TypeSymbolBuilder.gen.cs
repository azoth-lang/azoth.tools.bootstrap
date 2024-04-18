using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal partial class TypeSymbolBuilder
{
    partial void StartRun();

    private partial SymbolBuilderContext EndRun(To.Package package);

    private partial To.Package Transform(To.Package from);
}
