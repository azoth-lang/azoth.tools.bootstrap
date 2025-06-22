using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.API;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Lab.Config;
using ExhaustiveMatching;
using Nito.AsyncEx;

namespace Azoth.Tools.Bootstrap.Lab.Build;

/// <summary>
/// Represents and determines an order to build packages in.
/// </summary>
internal class ProjectSet : IEnumerable<Project>
{
    private readonly Dictionary<string, Project> projects = [];

    public void AddAll(ProjectConfigSet configs)
    {
        foreach (var config in configs)
            GetOrAdd(config, configs);

        foreach (var (_, project) in projects)
            // Force dev references to be evaluated
            _ = project.DevReferences;
    }

    private Project Get(ProjectConfig config)
    {
        var projectDir = GetDirectoryName(config);
        return projects[projectDir];
    }

    private Project GetOrAdd(ProjectConfig config, ProjectConfigSet configs)
    {
        var projectDir = GetDirectoryName(config);
        if (projects.TryGetValue(projectDir, out var existingProject))
            return existingProject;

        // Add a placeholder to prevent cycles (safe because we will replace it below)
        projects.Add(projectDir, null!);

        var references = CreateReferences(config, minimumRelation: ProjectRelation.Internal).ToList();
        var devReferences = Lazy.Create(() => CreateReferences(config, ProjectRelation.Dev).ToFixedList());
        var project = new Project(config, references, devReferences);
        projects[projectDir] = project;
        return project;

        IEnumerable<ProjectReference> CreateReferences(ProjectConfig projectConfig, ProjectRelation minimumRelation)
            // TODO be more exact about selecting distinct references or possibly even merging them
            => projectConfig.Dependencies!.Where(p => p.Value?.Relation > minimumRelation)
                .SelectMany(CreateReferencesForDependency).DistinctBy(r => r.Project.Name);

        IEnumerable<ProjectReference> CreateReferencesForDependency(string alias, ProjectDependencyConfig? dependencyConfig)
        {
            // Note: these are input validations
            // TODO do these validations earlier
            if (dependencyConfig is null)
                throw new InvalidOperationException("Dependency must not be null.");
            if (dependencyConfig.Relation == ProjectRelation.None)
                throw new InvalidOperationException("None is not a valid relation.");
            var dependencyProjectConfig = configs[config, alias];
            var dependencyProject = GetOrAdd(dependencyProjectConfig, configs)
                ?? throw new InvalidOperationException("Dependency cycle detected.");
            var isTrusted = dependencyConfig.Trusted ?? throw new InvalidOperationException();
            yield return new(alias, dependencyProject, isTrusted, dependencyConfig.Relation, dependencyConfig.Bundle);
            foreach (var bundledReference in dependencyProject.References.Where(r => r.Bundle != ProjectRelation.None))
            {
                yield return new(bundledReference.Project.Name, bundledReference.Project,
                    isTrusted && bundledReference.IsTrusted, bundledReference.Bundle, ProjectRelation.None);
            }
        }
    }

    private static string GetDirectoryName(ProjectConfig config)
        => Path.GetDirectoryName(config.FullPath) ?? throw new InvalidOperationException("Null directory name");

    public IEnumerator<Project> GetEnumerator() => projects.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task BuildAsync(TaskScheduler taskScheduler, bool verbose)
    {
        try
        {
            await ProcessProjects(taskScheduler, verbose, outputTests: false, BuildAsync, null);
        }
        catch (FatalCompilationErrorException)
        {
            // Errors already output, nothing to do
        }
    }

    private delegate Task<IPackageNode> ProcessAsync(
        AzothCompiler compiler,
        Project project,
        bool outputTests,
        Task<FixedDictionary<Project, Task<IPackageNode>>> projectBuildsTask,
        AsyncLock consoleLock);

    private async Task<(IPackageNode, IFixedSet<IPackageNode>)?> ProcessProjects(
        TaskScheduler taskScheduler,
        bool verbose,
        bool outputTests,
        ProcessAsync processAsync,
        ProjectConfig? entryProjectConfig)
    {
        _ = verbose; // verbose parameter will be needed in the future
        var taskFactory = new TaskFactory(taskScheduler);
        var projectBuilds = new Dictionary<Project, Task<IPackageNode>>();

        var projectBuildsSource = new TaskCompletionSource<FixedDictionary<Project, Task<IPackageNode>>>();
        var projectBuildsTask = projectBuildsSource.Task;

        // Sort projects to detect cycles, and so we can assume the tasks already exist
        var sortedProjects = TopologicalSort();
        var compiler = new AzothCompiler();
        var consoleLock = new AsyncLock();
        foreach (var project in sortedProjects)
        {
            var buildTask = taskFactory.StartNew(() => processAsync(compiler, project, outputTests, projectBuildsTask, consoleLock))
                                       .Unwrap(); // Needed because StartNew doesn't work intuitively with Async methods
            if (!projectBuilds.TryAdd(project, buildTask))
                throw new Exception("Project added to build set twice");
        }

        projectBuildsSource.SetResult(projectBuilds.ToFixedDictionary());

        var allBuilds = await Task.WhenAll(projectBuilds.Values).ConfigureAwait(false);

        if (entryProjectConfig is null) return null;

        var entryProject = Get(entryProjectConfig);
        var entryPackage = await projectBuilds[entryProject];
        var referencedPackages = allBuilds.WhereNotNull().Except(entryPackage).ToFixedSet();
        return (entryPackage, referencedPackages);
    }

    public async Task InterpretAsync(TaskScheduler taskScheduler, bool verbose, ProjectConfig entryProjectConfig)
    {
        try
        {
            var (entryPackageNode, referencedPackages) = (await ProcessProjects(taskScheduler, verbose,
                outputTests: false, CompileAsync, entryProjectConfig))!.Value;
            var interpreter = new AzothTreeInterpreter();
            var process = interpreter.Execute(entryPackageNode, referencedPackages);
            while (await process.StandardOutput.ReadLineAsync() is { } line) Console.WriteLine(line);
            await process.WaitForExitAsync();
            var stderr = await process.StandardError.ReadToEndAsync();
            await Console.Error.WriteLineAsync(stderr);
            Console.WriteLine($"Run took {process.RunTime}");
        }
        catch (FatalCompilationErrorException)
        {
            // Errors already output, nothing to do
        }
    }

    public async Task TestAsync(TaskScheduler taskScheduler, bool verbose, ProjectConfig testProjectConfig)
    {
        try
        {
            var (testPackageNode, referencedPackages) =
                (await ProcessProjects(taskScheduler, verbose, outputTests: true, CompileAsync, testProjectConfig))!.Value;
            var interpreter = new AzothTreeInterpreter();
            var process = interpreter.ExecuteTests(testPackageNode, referencedPackages);
            while (await process.StandardOutput.ReadLineAsync() is { } line) Console.WriteLine(line);
            await process.WaitForExitAsync();
            var stderr = await process.StandardError.ReadToEndAsync();
            await Console.Error.WriteLineAsync(stderr);
        }
        catch (FatalCompilationErrorException)
        {
            // Errors already output, nothing to do
        }
    }

    private static async Task<IPackageNode> BuildAsync(
        AzothCompiler compiler,
        Project project,
        bool outputTests,
        Task<FixedDictionary<Project, Task<IPackageNode>>> projectBuildsTask,
        AsyncLock consoleLock)
    {
        var package = await CompileAsync(compiler, project, outputTests, projectBuildsTask, consoleLock);
        var cacheDir = PrepareCacheDir(project);
        var codePath = EmitIL(project, package, outputTests, cacheDir);

        using (await consoleLock.LockAsync())
        {
            Console.WriteLine($"Build SUCCEEDED {project.Name} ({project.Path})");
        }

        return package;
    }

    private static async Task<IPackageNode> CompileAsync(
        AzothCompiler compiler,
        Project project,
        bool outputTests,
        Task<FixedDictionary<Project, Task<IPackageNode>>> projectBuildsTask,
        AsyncLock consoleLock)
    {
        // Doesn't affect compilation, only IL emitting
        _ = outputTests;

        var projectBuilds = await projectBuildsTask.ConfigureAwait(false);
        var sourceDir = Path.Combine(project.Path, "src");
        var sourcePaths = Directory.EnumerateFiles(sourceDir, "*.az", SearchOption.AllDirectories);
        var testSourcePaths = Directory.EnumerateFiles(sourceDir, "*.azt", SearchOption.AllDirectories);
        var symbolLoader = new PackageSymbolLoader(projectBuilds.ToFixedDictionary(e => (IdentifierName)e.Key.Name, e => e.Value));
        var references = project.References.Select(r => r.ToPackageReference()).WhereNotNull();

        using (await consoleLock.LockAsync())
        {
            Console.WriteLine($"Compiling {project.Name} ({project.Path})...");
        }
        var codeFiles = CreateCodePaths(sourcePaths, isTest: false);
        var testCodeFiles = CreateCodePaths(testSourcePaths, isTest: true);
        try
        {
            var package = await compiler.CompilePackageAsync(project.Name, codeFiles, testCodeFiles, references, symbolLoader);

            if (OutputDiagnostics(project, package.Diagnostics, consoleLock))
                return package;

            using (await consoleLock.LockAsync())
            {
                Console.WriteLine($"Compile SUCCEEDED {project.Name} ({project.Path})");
            }

            return package;
        }
        catch (FatalCompilationErrorException ex)
        {
            OutputDiagnostics(project, ex.Diagnostics, consoleLock);
            throw;
        }

        IEnumerable<CodePath> CreateCodePaths(IEnumerable<string> paths, bool isTest)
            => paths.Select(p => CreateCodePath(p, sourceDir, project.RootNamespace, isTest));

        static CodePath CreateCodePath(
            string path,
            string sourceDir,
            IFixedList<string> rootNamespace,
            bool isTest)
        {
            var relativeDirectory = Path.GetDirectoryName(Path.GetRelativePath(sourceDir, path))
                                    ?? throw new InvalidOperationException("Null directory name");
            var ns = rootNamespace.Concat(relativeDirectory.SplitOrEmpty(Path.DirectorySeparatorChar)).ToFixedList();
            return new(path, ns, isTest);
        }
    }

    private static string PrepareCacheDir(Project project)
    {
        var cacheDir = Path.Combine(project.Path, ".forge-cache");
        Directory.CreateDirectory(cacheDir); // Ensure the cache directory exists

        // Clear the cache directory?
        var dir = new DirectoryInfo(cacheDir);
        foreach (var file in dir.EnumerateFiles())
            file.Delete();
        foreach (var subDirectory in dir.EnumerateDirectories())
            subDirectory.Delete(true);

        return cacheDir;
    }

    private static bool OutputDiagnostics(Project project, DiagnosticCollection diagnostics, AsyncLock consoleLock)
    {
        if (diagnostics.IsEmpty)
            return false;
        using (consoleLock.Lock())
        {
            Console.WriteLine($"Build FAILED {project.Name} ({project.Path})");
            foreach (var group in diagnostics.GroupBy(d => d.File))
            {
                var fileDiagnostics = group.ToList();
                foreach (var diagnostic in fileDiagnostics.Take(10))
                {
                    Console.WriteLine(
                        $"{diagnostic.File.Reference}:{diagnostic.StartPosition.Line}:{diagnostic.StartPosition.Column} {diagnostic.Level} {diagnostic.ErrorCode}");
                    Console.WriteLine("    " + diagnostic.Message);
                }

                if (fileDiagnostics.Count > 10)
                {
                    Console.WriteLine($"{group.Key.Reference}");
                    Console.WriteLine(
                        $"    {fileDiagnostics.Skip(10).Count(d => d.Level >= DiagnosticLevel.CompilationError)} more errors not shown.");
                    Console.WriteLine(
                        $"    {fileDiagnostics.Skip(10).Count(d => d.Level == DiagnosticLevel.Warning)} more warnings not shown.");
                }
            }
        }

        return true;
    }

    private static string EmitIL(Project project, IPackageNode package, bool outputTests, string cacheDir)
    {
#pragma warning disable IDE0022
        throw new NotImplementedException();
#pragma warning restore IDE0022
        //var emittedPackages = new HashSet<PackageIL>();
        //var packagesToEmit = new Queue<PackageIL>();
        //packagesToEmit.Enqueue(package);

        //string outputPath;
        //switch (project.Template)
        //{
        //    case ProjectTemplate.App:
        //    {
        //        outputPath = Path.Combine(cacheDir, "program.azx");
        //    }
        //    break;
        //    case ProjectTemplate.Lib:
        //    {
        //        outputPath = Path.Combine(cacheDir, "lib.azil");
        //    }
        //    break;
        //    default:
        //        throw ExhaustiveMatch.Failed(project.Template);
        //}

        //var codeEmitter = new ILEmitter();
        //while (packagesToEmit.TryDequeue(out var currentPackage))
        //{
        //    if (!emittedPackages.Contains(currentPackage))
        //    {
        //        codeEmitter.Emit(currentPackage);
        //        emittedPackages.Add(currentPackage);
        //        packagesToEmit.EnqueueRange(currentPackage.References);
        //    }
        //}



        //File.WriteAllText(outputPath, codeEmitter.GetEmittedCode(), Encoding.UTF8);

        //return outputPath;
    }

    private List<Project> TopologicalSort()
    {
        var projectAlive = projects.Values.ToDictionary(p => p, _ => SortState.Unvisited);
        var sorted = new List<Project>(projects.Count);
        foreach (var project in projects.Values)
            TopologicalSortVisit(project, projectAlive, sorted);

        return sorted;
    }

    private static void TopologicalSortVisit(
        Project project,
        Dictionary<Project, SortState> state, List<Project> sorted)
    {
        switch (state[project])
        {
            case SortState.Visited:
                return;

            case SortState.Visiting:// Cycle
                throw new Exception("Dependency Cycle");

            case SortState.Unvisited:
                state[project] = SortState.Visiting;
                foreach (var referencedProject in project.References.Select(r => r.Project))
                    TopologicalSortVisit(referencedProject, state, sorted);
                state[project] = SortState.Visited;
                sorted.Add(project);
                return;

            default:
                throw ExhaustiveMatch.Failed(state[project]);
        }
    }

    private enum SortState
    {
        Unvisited,
        Visiting,
        Visited,
    }
}
