using System;
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

    public ValueTask<IPackageFacetNode> CompilePackageFacetAsync(
        IdentifierName name,
        FacetKind facet,
        IEnumerable<ICodeFileSource> files,
        IEnumerable<PackageReference> references,
        IPackageSymbolLoader symbolLoader)
        => CompilePackageFacetAsync(name, facet, files, references, symbolLoader, TaskScheduler.Default);

    public async ValueTask<IPackageFacetNode> CompilePackageFacetAsync(
        IdentifierName name,
        FacetKind facet,
        IEnumerable<ICodeFileSource> fileSources,
        IEnumerable<PackageReference> references,
        IPackageSymbolLoader symbolLoader,
        TaskScheduler taskScheduler)
    {
        var referencesSyntax = references.Select(r => r.ToSyntax()).ToFixedSet();
        if (referencesSyntax.Any(r => r.PackageName == name))
            throw new ArgumentException("Package cannot reference itself.", nameof(references));
        var compilationUnits = await ParseFilesAsync(fileSources, taskScheduler);
        var packageFacetSyntax = IPackageFacetSyntax.Create(name, facet, compilationUnits, referencesSyntax);
        var analyzer = new SemanticAnalyzer(symbolLoader);
        return await analyzer.CheckAsync(packageFacetSyntax);
    }

    private async ValueTask<IFixedSet<ICompilationUnitSyntax>> ParseFilesAsync(IEnumerable<ICodeFileSource> codeFileSources, TaskScheduler taskScheduler)
        // TODO use task scheduler?
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
