using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.NotImplemented;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing
{
    public class CompilationUnitParser
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Entry point class which may need to be non-static in future")]
        public ICompilationUnitSyntax Parse(ITokenIterator<IEssentialToken> tokens)
        {
            var implicitNamespaceName = ParseImplicitNamespaceName(tokens);
            var parser = new Parser(tokens, implicitNamespaceName);
            var usingDirectives = parser.ParseUsingDirectives();
            var declarations = parser.ParseNonMemberDeclarations<IEndOfFileToken>();
            var eof = tokens.Required<IEndOfFileToken>();
            var span = TextSpan.FromStartEnd(0, eof.End);
            var diagnostics = tokens.Context.Diagnostics;
            var compilationUnit = new CompilationUnitSyntax(implicitNamespaceName, span,
                tokens.Context.File, usingDirectives,
                declarations);

            CheckSyntax(compilationUnit, diagnostics);
            compilationUnit.Attach(diagnostics.Build());
            return compilationUnit;
        }

        private static NamespaceName ParseImplicitNamespaceName(ITokenIterator<IEssentialToken> tokens)
        {
            NamespaceName name = NamespaceName.Global;
            foreach (var segment in tokens.Context.File.Reference.Namespace)
                name = name.Qualify(segment);

            return name;
        }

        private static void CheckSyntax(CompilationUnitSyntax compilationUnit, Diagnostics diagnostics)
        {
            var notImplementedChecker = new SyntaxNotImplementedChecker(compilationUnit, diagnostics);
            notImplementedChecker.Check();
        }
    }
}
