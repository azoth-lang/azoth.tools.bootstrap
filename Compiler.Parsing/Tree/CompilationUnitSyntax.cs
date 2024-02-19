using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class CompilationUnitSyntax : Syntax, ICompilationUnitSyntax
{
    public CodeFile File { get; }
    public NamespaceName ImplicitNamespaceName { get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    public IFixedList<INonMemberDeclarationSyntax> Declarations { get; }
    public IFixedList<Diagnostic> Diagnostics { get; private set; }

    public CompilationUnitSyntax(
        NamespaceName implicitNamespaceName,
        TextSpan span,
        CodeFile file,
        FixedList<IUsingDirectiveSyntax> usingDirectives,
        IFixedList<INonMemberDeclarationSyntax> declarations)
        : base(span)
    {
        File = file;
        ImplicitNamespaceName = implicitNamespaceName;
        UsingDirectives = usingDirectives;
        Declarations = declarations;
        Diagnostics = FixedList<Diagnostic>.Empty;
    }

    public void Attach(IFixedList<Diagnostic> diagnostics)
        => Diagnostics = diagnostics;

    public override string ToString() => File.Reference.ToString();
}
