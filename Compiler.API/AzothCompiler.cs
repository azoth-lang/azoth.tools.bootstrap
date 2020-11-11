using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.API
{
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

        public Task<PackageIL> CompilePackageAsync(
            Name name,
            IEnumerable<ICodeFileSource> files,
            FixedDictionary<Name, Task<PackageIL>> referenceTasks)
        {
            return CompilePackageAsync(name, files, referenceTasks, TaskScheduler.Default);
        }

        public async Task<PackageIL> CompilePackageAsync(
            Name name,
            IEnumerable<ICodeFileSource> fileSources,
            FixedDictionary<Name, Task<PackageIL>> referenceTasks,
            TaskScheduler taskScheduler)
        {
            var lexer = new Lexer();
            var parser = new CompilationUnitParser();
            var parseBlock = new TransformBlock<ICodeFileSource, ICompilationUnitSyntax>(
                async (fileSource) =>
                {
                    var file = await fileSource.LoadAsync().ConfigureAwait(false);
                    var context = new ParseContext(file, new Diagnostics());
                    var tokens = lexer.Lex(context).WhereNotTrivia();
                    return parser.Parse(tokens);
                }, new ExecutionDataflowBlockOptions()
                {
                    TaskScheduler = taskScheduler,
                    EnsureOrdered = false,
                });

            foreach (var fileSource in fileSources)
                parseBlock.Post(fileSource);

            parseBlock.Complete();

            await parseBlock.Completion.ConfigureAwait(false);

            if (!parseBlock.TryReceiveAll(out var compilationUnits))
                throw new Exception("Not all compilation units are ready");

            var referencePairs = await Task
                                       .WhenAll(referenceTasks.Select(async kv =>
                                           (alias: kv.Key, package: await kv.Value.ConfigureAwait(false))))
                                       .ConfigureAwait(false);
            var references = referencePairs.ToFixedDictionary(r => r.alias, r => r.package);

            // TODO add the references to the package syntax
            var packageSyntax = new PackageSyntax(name, compilationUnits.ToFixedSet(), references);

            var analyzer = new SemanticAnalyzer()
            {
                SaveLivenessAnalysis = SaveLivenessAnalysis,
                SaveReachabilityGraphs = SaveReachabilityGraphs,
            };

            return analyzer.Check(packageSyntax);
        }

        public PackageIL CompilePackage(
            string name,
            IEnumerable<ICodeFileSource> fileSources,
            FixedDictionary<Name, PackageIL> references)
        {
            return CompilePackage(name, fileSources.Select(s => s.Load()), references);
        }

        public PackageIL CompilePackage(
            string name,
            IEnumerable<CodeFile> files,
            FixedDictionary<Name, PackageIL> references)
        {
            var lexer = new Lexer();
            var parser = new CompilationUnitParser();
            var compilationUnits = files
                .Select(file =>
                {
                    var context = new ParseContext(file, new Diagnostics());
                    var tokens = lexer.Lex(context).WhereNotTrivia();
                    return parser.Parse(tokens);
                })
                .ToFixedSet();
            var packageSyntax = new PackageSyntax(name, compilationUnits, references);

            var analyzer = new SemanticAnalyzer()
            {
                SaveLivenessAnalysis = SaveLivenessAnalysis,
                SaveReachabilityGraphs = SaveReachabilityGraphs,
            };

            return analyzer.Check(packageSyntax);
        }
    }
}
