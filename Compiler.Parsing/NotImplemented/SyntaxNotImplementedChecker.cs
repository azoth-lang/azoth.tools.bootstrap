using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.NotImplemented;

/// <summary>
/// Reports errors for syntax that the parsing supports but the semantic
/// analyzer doesn't.
/// </summary>
internal class SyntaxNotImplementedChecker
{
    private readonly ICompilationUnitSyntax compilationUnit;
    private readonly DiagnosticCollectionBuilder diagnostics;
    private readonly CodeFile file;

    public SyntaxNotImplementedChecker(ICompilationUnitSyntax compilationUnit, DiagnosticCollectionBuilder diagnostics)
    {
        this.compilationUnit = compilationUnit;
        this.diagnostics = diagnostics;
        file = compilationUnit.File;
    }

    public void Check() => Check(compilationUnit);

    protected void Check(ISyntax syntax)
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

        foreach (var child in syntax.Children())
            Check(child);
    }
}
