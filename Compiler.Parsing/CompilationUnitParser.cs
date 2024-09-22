using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.NotImplemented;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public class CompilationUnitParser
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Entry point class which may need to be non-static in future")]
    public ICompilationUnitSyntax Parse(ITokenIterator<IEssentialToken> tokens)
    {
        var implicitNamespaceName = ParseImplicitNamespaceName(tokens);
        var parser = new Parser(tokens);
        var importDirectives = parser.ParseImportDirectives();
        var declarations = parser.ParseNamespaceBlockMemberDefinitions<IEndOfFileToken>();
        var eof = tokens.Required<IEndOfFileToken>();
        var span = TextSpan.FromStartEnd(0, eof.End);
        var diagnostics = tokens.Context.Diagnostics;
        var compilationUnit = ICompilationUnitSyntax.Create(span, tokens.Context.File,
            implicitNamespaceName, DiagnosticCollection.Empty, importDirectives, declarations);

        CheckSyntax(compilationUnit, diagnostics);
        return compilationUnit.With(diagnostics.Build());
    }

    private static NamespaceName ParseImplicitNamespaceName(ITokenIterator<IEssentialToken> tokens)
    {
        NamespaceName name = NamespaceName.Global;
        foreach (var segment in tokens.Context.File.Reference.Namespace)
            name = name.Qualify(segment);

        return name;
    }

    private static void CheckSyntax(ICompilationUnitSyntax compilationUnit, DiagnosticCollectionBuilder diagnostics)
    {
        var notImplementedChecker = new SyntaxNotImplementedChecker(compilationUnit, diagnostics);
        notImplementedChecker.Check();
    }
}
