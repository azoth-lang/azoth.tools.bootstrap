using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal partial class TypeSymbolBuilder
{
    private TypeSymbolBuilder(SymbolBuilderContext context)
    {
        throw new System.NotImplementedException();
    }

    private partial SymbolBuilderContext EndRun(To.Package to)
        => throw new System.NotImplementedException();

    private partial To.Package TransformPackage(From.Package from)
        => throw new System.NotImplementedException();
}
