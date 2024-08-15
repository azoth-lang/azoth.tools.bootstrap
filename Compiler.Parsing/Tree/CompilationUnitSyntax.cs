using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class CompilationUnitSyntax : CodeSyntax, ICompilationUnitSyntax
{
    public CodeFile File { get; }
    public NamespaceName ImplicitNamespaceName { get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    public IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
    public DiagnosticCollection Diagnostics { get; private set; }

    public CompilationUnitSyntax(
        NamespaceName implicitNamespaceName,
        TextSpan span,
        CodeFile file,
        IFixedList<IUsingDirectiveSyntax> usingDirectives,
        IFixedList<INonMemberDefinitionSyntax> declarations)
        : base(span)
    {
        File = file;
        ImplicitNamespaceName = implicitNamespaceName;
        UsingDirectives = usingDirectives;
        Definitions = declarations;
        Diagnostics = DiagnosticCollection.Empty;
    }

    public void Attach(DiagnosticCollection diagnostics)
        => Diagnostics = diagnostics;

    public override string ToString() => File.Reference.ToString();
}
