using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.API;

// TODO add support in the API for handling multiple projects and assembling facets into packages
public class AzothCompiler
{
    private readonly Lexer lexer = new();
    private readonly CompilationUnitParser parser = new();

    public Task<IPackageNode> CompilePackageAsync(
        IdentifierName name,
        IEnumerable<ICodeFileSource> files,
        IEnumerable<ICodeFileSource> testingFileSources,
        IEnumerable<PackageReference> references,
        IPackageSymbolLoader symbolLoader)
        => CompilePackageAsync(name, files, testingFileSources, references, symbolLoader, TaskScheduler.Default);

    // TODO replace with CompilePackageFacetAsync
    // TODO change references into a proper representation of the syntax
    // TODO add a parameter for loading symbols for references
    public async Task<IPackageNode> CompilePackageAsync(
        IdentifierName name,
        IEnumerable<ICodeFileSource> fileSources,
        IEnumerable<ICodeFileSource> testingFileSources,
        IEnumerable<PackageReference> references,
        IPackageSymbolLoader symbolLoader,
        TaskScheduler taskScheduler)
    {
        var compilationUnits = await ParseFilesAsync(fileSources);
        var testingCompilationUnits = await ParseFilesAsync(testingFileSources);
        var referenceSyntax = references.Select(r => r.ToSyntax()).ToFixedSet();

        var packageMainSyntax = IPackageFacetSyntax.Create(name, FacetKind.Main, compilationUnits, referenceSyntax);
        var packageTestsSyntax = IPackageFacetSyntax.Create(name, FacetKind.Tests, testingCompilationUnits, referenceSyntax);

        var analyzer = new SemanticAnalyzer(symbolLoader);
        return await analyzer.CheckAsync(packageMainSyntax, packageTestsSyntax);
    }

    private async Task<IFixedSet<ICompilationUnitSyntax>> ParseFilesAsync(IEnumerable<ICodeFileSource> codeFileSources)
        // TODO manage degree of parallelism
        // TODO Task.WhenAll has issues when errors and cancellation occur
        => (await Task.WhenAll(codeFileSources.Select(ParseFileAsync))).ToFixedSet();

    private async Task<ICompilationUnitSyntax> ParseFileAsync(ICodeFileSource fileSource)
    {
        var file = await fileSource.LoadAsync().ConfigureAwait(false);
        var tokens = lexer.Lex(new ParseContext(file)).WhereNotTrivia();
        return parser.Parse(tokens);
    }
}
