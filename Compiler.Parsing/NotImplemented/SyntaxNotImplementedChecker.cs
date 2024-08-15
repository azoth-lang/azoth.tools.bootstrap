using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Syntax.Walkers;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.NotImplemented;

/// <summary>
/// Reports errors for syntax that the parsing supports but the semantic
/// analyzer doesn't.
/// </summary>
internal class SyntaxNotImplementedChecker : SyntaxWalker
{
    private readonly CompilationUnitSyntax compilationUnit;
    private readonly DiagnosticCollectionBuilder diagnostics;
    private readonly CodeFile file;

    public SyntaxNotImplementedChecker(CompilationUnitSyntax compilationUnit, DiagnosticCollectionBuilder diagnostics)
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
            case IConstructorDefinitionSyntax syn:
                if (syn.Name is not null)
                    diagnostics.Add(ParseError.NotImplemented(file, syn.Span, "Named constructors"));
                break;
            case IFieldDefinitionSyntax syn:
                if (syn.Initializer is not null)
                    diagnostics.Add(ParseError.NotImplemented(file, syn.Initializer.Span, "Field initializers"));
                break;
        }
        WalkChildren(syntax);
    }
}
