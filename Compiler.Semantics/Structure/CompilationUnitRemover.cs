using System.Collections.Generic;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal partial class CompilationUnitRemover
{
    private partial To.Package Transform(From.Package from)
        => Create(from, Transform(from.CompilationUnits), Transform(from.TestingCompilationUnits));

    private partial IEnumerable<To.NamespaceMemberDeclaration> Transform(From.CompilationUnit from)
        => Transform(from.Declarations, from.File);
}
