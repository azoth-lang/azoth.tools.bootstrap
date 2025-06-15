using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public class AzothCompiler
{
    /// <summary>
    /// Whether to store the borrow checker claims for each function and method.
    /// Default Value: false
    /// </summary>
    public bool SaveReachabilityGraphs { get; set; }

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

        // TODO add the references to the package syntax
        var packageSyntax = IPackageSyntax.Create(name, compilationUnits, testingCompilationUnits, referenceSyntax);

        var analyzer = new SemanticAnalyzer()
        {
            SaveReachabilityGraphs = SaveReachabilityGraphs,
        };

        return analyzer.Check(packageSyntax);

        async Task<IFixedSet<ICompilationUnitSyntax>> ParseFilesAsync(IEnumerable<ICodeFileSource> codeFileSources)
        {
            var parseBlock = new TransformBlock<ICodeFileSource, ICompilationUnitSyntax>(async fileSource =>
            {
                var file = await fileSource.LoadAsync().ConfigureAwait(false);
                var tokens = lexer.Lex(new ParseContext(file)).WhereNotTrivia();
                return parser.Parse(tokens);
            }, new() { TaskScheduler = taskScheduler, EnsureOrdered = false, });

            foreach (var fileSource in codeFileSources) parseBlock.Post(fileSource);

            parseBlock.Complete();

            await parseBlock.Completion.ConfigureAwait(false);

            if (!parseBlock.TryReceiveAll(out var compilationUnits))
                throw new Exception("Not all compilation units are ready");

            return compilationUnits.ToFixedSet();
        }
    }

    public IPackageNode CompilePackage(
        string name,
        IEnumerable<ICodeFileSource> fileSources,
        IEnumerable<ICodeFileSource> testingFileSources,
        IEnumerable<PackageReference> references)
        => CompilePackage(name, fileSources.Select(s => s.Load()), testingFileSources.Select(s => s.Load()), references);

    public IPackageNode CompilePackage(
        string name,
        IEnumerable<CodeFile> files,
        IEnumerable<CodeFile> testingFiles,
        IEnumerable<PackageReference> references)
    {
        var lexer = new Lexer();
        var parser = new CompilationUnitParser();
        var compilationUnits = ParseFiles(files);
        var testingCompilationUnits = ParseFiles(testingFiles);
        var referenceSyntax = references.Select(r => r.ToSyntax()).ToFixedSet();
        var packageSyntax = IPackageSyntax.Create(name, compilationUnits, testingCompilationUnits, referenceSyntax);

        var analyzer = new SemanticAnalyzer()
        {
            SaveReachabilityGraphs = SaveReachabilityGraphs,
        };

        return analyzer.Check(packageSyntax);

        IFixedSet<ICompilationUnitSyntax> ParseFiles(IEnumerable<CodeFile> codeFiles)
        {
            return codeFiles
                   .Select(file =>
                   {
                       var tokens = lexer.Lex(new ParseContext(file)).WhereNotTrivia();
                       return parser.Parse(tokens);
                   })
                   .ToFixedSet();
        }
    }
}
