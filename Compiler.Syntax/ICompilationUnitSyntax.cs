using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface ICompilationUnitSyntax
{
    public ICompilationUnitSyntax With(DiagnosticCollection diagnostics)
        => Create(Span, File, ImplicitNamespaceName, diagnostics, ImportDirectives, Definitions);
}
