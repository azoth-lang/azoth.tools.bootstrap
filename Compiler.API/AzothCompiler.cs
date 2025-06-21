using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public class AzothCompiler
{
    public Task<IPackageNode> CompilePackageAsync(
        IdentifierName name,
        IEnumerable<ICodeFileSource> files,
        IEnumerable<ICodeFileSource> testingFileSources,
        IEnumerable<PackageReferenceAsync> references)
        => CompilePackageAsync(name, files, testingFileSources, references, TaskScheduler.Default);

    public async Task<IPackageNode> CompilePackageAsync(
        IdentifierName name,
        IEnumerable<ICodeFileSource> fileSources,
        IEnumerable<ICodeFileSource> testingFileSources,
        IEnumerable<PackageReferenceAsync> references,
        TaskScheduler taskScheduler)
    {
        var lexer = new Lexer();
        var parser = new CompilationUnitParser();
        var compilationUnits = await ParseFilesAsync(fileSources);
        var testingCompilationUnits = await ParseFilesAsync(testingFileSources);
        var referenceSyntax = (await Task.WhenAll(
            references.Select(r => r.ToSyntaxAsync())).ConfigureAwait(false)).ToFixedSet();

        var packageMainSyntax = IPackageFacetSyntax.Create(name, FacetKind.Main, compilationUnits, referenceSyntax);
        var packageTestsSyntax = IPackageFacetSyntax.Create(name, FacetKind.Tests, testingCompilationUnits, referenceSyntax);

        var analyzer = new SemanticAnalyzer();
        return analyzer.Check(packageMainSyntax, packageTestsSyntax);

        async Task<IFixedSet<ICompilationUnitSyntax>> ParseFilesAsync(IEnumerable<ICodeFileSource> codeFileSources)
        {
            // TODO manage degree of parallelism
            return (await Task.WhenAll(codeFileSources.Select(ParseFileAsync))).ToFixedSet();
        }

        async Task<ICompilationUnitSyntax> ParseFileAsync(ICodeFileSource fileSource)
        {
            var file = await fileSource.LoadAsync().ConfigureAwait(false);
            var tokens = lexer.Lex(new ParseContext(file)).WhereNotTrivia();
            return parser.Parse(tokens);
        }
    }
}
