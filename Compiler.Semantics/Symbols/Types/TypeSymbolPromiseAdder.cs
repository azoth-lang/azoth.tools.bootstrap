using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal partial class TypeSymbolPromiseAdder
{
    private partial To.TypeDeclaration Transform(From.TypeDeclaration from)
        => Create(from, new AcyclicPromise<UserTypeSymbol>());
}
