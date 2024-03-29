using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.NotImplemented;

/// <summary>
/// Reports errors for syntax that the parsing supports but the semantic
/// analyzer doesn't.
/// </summary>
internal class SyntaxNotImplementedChecker : SyntaxWalker
{
    private readonly CompilationUnitSyntax compilationUnit;
    private readonly Diagnostics diagnostics;
    private readonly CodeFile file;

    public SyntaxNotImplementedChecker(CompilationUnitSyntax compilationUnit, Diagnostics diagnostics)
    {
        this.compilationUnit = compilationUnit;
        this.diagnostics = diagnostics;
        file = compilationUnit.File;
    }

    public void Check() => WalkNonNull(compilationUnit);

    protected override void WalkNonNull(ISyntax syntax)
    {
        switch (syntax)
        {
            case INamedParameterSyntax syn:
                if (syn.DefaultValue is not null)
                    diagnostics.Add(ParseError.NotImplemented(file, syn.DefaultValue.Span, "Default values"));
                break;
            case IFieldParameterSyntax syn:
                if (syn.DefaultValue is not null)
                    diagnostics.Add(ParseError.NotImplemented(file, syn.DefaultValue.Span, "Default values"));
                break;
            case IConstructorDeclarationSyntax syn:
                if (syn.Name is not null)
                    diagnostics.Add(ParseError.NotImplemented(file, syn.Span, "Named constructors"));
                break;
            case IFieldDeclarationSyntax syn:
                if (syn.Initializer is not null)
                    diagnostics.Add(ParseError.NotImplemented(file, syn.Initializer.Span, "Field initializers"));
                break;
        }
        WalkChildren(syntax);
    }
}
