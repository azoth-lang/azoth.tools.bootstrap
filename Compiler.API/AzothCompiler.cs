using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.API;

public class AzothCompiler
{
    /// <summary>
    /// Whether to store the liveness analysis for each function and method.
    /// Default Value: false
    /// </summary>
    public bool SaveLivenessAnalysis { get; set; }

    /// <summary>
    /// Whether to store the borrow checker claims for each function and method.
    /// Default Value: false
    /// </summary>
    public bool SaveReachabilityGraphs { get; set; }

    public Task<Package> CompilePackageAsync(
        IdentifierName name,
        IEnumerable<ICodeFileSource> files,
        IEnumerable<ICodeFileSource> testingFileSources,
        FixedDictionary<IdentifierName, Task<Package>> referenceTasks)
        => CompilePackageAsync(name, files, testingFileSources, referenceTasks, TaskScheduler.Default);

    public async Task<Package> CompilePackageAsync(
        IdentifierName name,
        IEnumerable<ICodeFileSource> fileSources,
        IEnumerable<ICodeFileSource> testingFileSources,
        FixedDictionary<IdentifierName, Task<Package>> referenceTasks,
        TaskScheduler taskScheduler)
    {
        var lexer = new Lexer();
        var parser = new CompilationUnitParser();
        var compilationUnits = await ParseFilesAsync(fileSources);
        var testingCompilationUnits = await ParseFilesAsync(testingFileSources);
        var referencePairs = await Task
                                   .WhenAll(referenceTasks.Select(async kv =>
                                       (alias: kv.Key, package: await kv.Value.ConfigureAwait(false))))
                                   .ConfigureAwait(false);
        var references = referencePairs.ToFixedDictionary(r => r.alias, r => r.package);

        // TODO add the references to the package syntax
        var packageSyntax = new PackageSyntax<Package>(name, compilationUnits, testingCompilationUnits, references);

        var analyzer = new SemanticAnalyzer()
        {
            SaveLivenessAnalysis = SaveLivenessAnalysis,
            SaveReachabilityGraphs = SaveReachabilityGraphs,
        };

        return analyzer.Check(packageSyntax);

        async Task<IFixedSet<ICompilationUnitSyntax>> ParseFilesAsync(IEnumerable<ICodeFileSource> codeFileSources)
        {
            var parseBlock = new TransformBlock<ICodeFileSource, ICompilationUnitSyntax>(async fileSource =>
            {
                var file = await fileSource.LoadAsync().ConfigureAwait(false);
                var context = new ParseContext(file, new Diagnostics());
                var tokens = lexer.Lex(context).WhereNotTrivia();
                return parser.Parse(tokens);
            }, new ExecutionDataflowBlockOptions() { TaskScheduler = taskScheduler, EnsureOrdered = false, });

            foreach (var fileSource in codeFileSources) parseBlock.Post(fileSource);

            parseBlock.Complete();

            await parseBlock.Completion.ConfigureAwait(false);

            if (!parseBlock.TryReceiveAll(out var compilationUnits))
                throw new Exception("Not all compilation units are ready");

            return compilationUnits.ToFixedSet();
        }
    }

    public Package CompilePackage(
        string name,
        IEnumerable<ICodeFileSource> fileSources,
        IEnumerable<ICodeFileSource> testingFileSources,
        FixedDictionary<IdentifierName, Package> references)
        => CompilePackage(name, fileSources.Select(s => s.Load()), testingFileSources.Select(s => s.Load()), references);

    public Package CompilePackage(
        string name,
        IEnumerable<CodeFile> files,
        IEnumerable<CodeFile> testingFiles,
        FixedDictionary<IdentifierName, Package> references)
    {
        var lexer = new Lexer();
        var parser = new CompilationUnitParser();
        var compilationUnits = ParseFiles(files);
        var testingCompilationUnits = ParseFiles(testingFiles);
        var packageSyntax = new PackageSyntax<Package>(name, compilationUnits, testingCompilationUnits, references);

        var analyzer = new SemanticAnalyzer()
        {
            SaveLivenessAnalysis = SaveLivenessAnalysis,
            SaveReachabilityGraphs = SaveReachabilityGraphs,
        };

        return analyzer.Check(packageSyntax);

        IFixedSet<ICompilationUnitSyntax> ParseFiles(IEnumerable<CodeFile> codeFiles)
        {
            return codeFiles
                   .Select(file =>
                   {
                       var context = new ParseContext(file, new Diagnostics());
                       var tokens = lexer.Lex(context).WhereNotTrivia();
                       return parser.Parse(tokens);
                   })
                   .ToFixedSet();
        }
    }
}
