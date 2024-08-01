using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.API;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Lab.Config;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Lab.Build;

/// <summary>
/// Represents and determines an order to build packages in.
/// </summary>
internal class ProjectSet : IEnumerable<Project>
{
    private readonly Dictionary<string, Project> projects = new();

    public void AddAll(ProjectConfigSet configs)
    {
        foreach (var config in configs)
            GetOrAdd(config, configs);
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
        var dependencies = config.Dependencies!.Select(d =>
        {
            var (name, config) = d;
            var dependencyConfig = configs[name];
            var dependencyProject = GetOrAdd(dependencyConfig, configs);
            return new ProjectReference(name, dependencyProject, config?.Trusted ?? throw new InvalidOperationException());
        }).ToList();

        var project = new Project(config, dependencies);
        projects[projectDir] = project;
        return project;
    }

    private static string GetDirectoryName(ProjectConfig config)
        => Path.GetDirectoryName(config.FullPath) ?? throw new InvalidOperationException("Null directory name");

    public IEnumerator<Project> GetEnumerator() => projects.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task BuildAsync(TaskScheduler taskScheduler, bool verbose)
        => await ProcessProjects(taskScheduler, verbose, outputTests: false, BuildAsync, null);

    private delegate Task<(Package, IPackageNode)?> ProcessAsync(
        AzothCompiler compiler,
        Project project,
        bool outputTests,
        Task<FixedDictionary<Project, Task<(Package, IPackageNode)?>>> projectBuildsTask,
        object consoleLock);

    private async Task<(Package, IPackageNode, IFixedSet<IPackageNode>)?> ProcessProjects(
        TaskScheduler taskScheduler,
        bool verbose,
        bool outputTests,
        ProcessAsync processAsync,
        ProjectConfig? entryProjectConfig)
    {
        _ = verbose; // verbose parameter will be needed in the future
        var taskFactory = new TaskFactory(taskScheduler);
        var projectBuilds = new Dictionary<Project, Task<(Package, IPackageNode)?>>();

        var projectBuildsSource = new TaskCompletionSource<FixedDictionary<Project, Task<(Package, IPackageNode)?>>>();
        var projectBuildsTask = projectBuildsSource.Task;

        // Sort projects to detect cycles and so we can assume the tasks already exist
        var sortedProjects = TopologicalSort();
        var compiler = new AzothCompiler();
        var consoleLock = new object();
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
        var entryPackageResult = await projectBuilds[entryProject];
        if (entryPackageResult is var (entryPackage, entryPackageNode))
        {
            var referencedPackages = allBuilds.WhereNotNull().Select(b => b.Item2).Except(entryPackageNode).ToFixedSet();
            return (entryPackage, entryPackageNode, referencedPackages);
        }
        return null;
    }

    public async Task InterpretAsync(TaskScheduler taskScheduler, bool verbose, ProjectConfig entryProjectConfig)
    {
        var packages = await ProcessProjects(taskScheduler, verbose, outputTests: false, CompileAsync, entryProjectConfig);

        if (packages is not var (entryPackage, entryPackageNode, referencedPackages))
            // Fatal Compile Errors
            return;

        var interpreter = new AzothTreeInterpreter();
        var process = interpreter.Execute(entryPackageNode, referencedPackages);
        await process.WaitForExitAsync();
        var stdout = await process.StandardOutput.ReadToEndAsync();
        Console.WriteLine(stdout);
        var stderr = await process.StandardError.ReadToEndAsync();
        await Console.Error.WriteLineAsync(stderr);
    }

    public async Task TestAsync(TaskScheduler taskScheduler, bool verbose, ProjectConfig testProjectConfig)
    {
        var packages = await ProcessProjects(taskScheduler, verbose, outputTests: true, CompileAsync, testProjectConfig);

        if (packages is not var (testPackage, testPackageNode, referencedPackages))
            // Fatal Compile Errors
            return;

        var interpreter = new AzothTreeInterpreter();
        var process = interpreter.ExecuteTests(testPackageNode, referencedPackages);
        await process.WaitForExitAsync();
        var stdout = await process.StandardOutput.ReadToEndAsync();
        Console.WriteLine(stdout);
        var stderr = await process.StandardError.ReadToEndAsync();
        await Console.Error.WriteLineAsync(stderr);
    }

    private static async Task<(Package, IPackageNode)?> BuildAsync(
        AzothCompiler compiler,
        Project project,
        bool outputTests,
        Task<FixedDictionary<Project, Task<(Package, IPackageNode)?>>> projectBuildsTask,
        object consoleLock)
    {
        var compileResult = await CompileAsync(compiler, project, outputTests, projectBuildsTask, consoleLock);
        if (compileResult is not var (package, packageNode)) return null;

        var cacheDir = PrepareCacheDir(project);
        var codePath = EmitIL(project, package, outputTests, cacheDir);

        lock (consoleLock)
        {
            Console.WriteLine($"Build SUCCEEDED {project.Name} ({project.Path})");
        }

        return (package, packageNode);
    }

    private static async Task<(Package, IPackageNode)?> CompileAsync(
        AzothCompiler compiler,
        Project project,
        bool outputTests,
        Task<FixedDictionary<Project, Task<(Package, IPackageNode)?>>> projectBuildsTask,
        object consoleLock)
    {
        // Doesn't affect compilation, only IL emitting
        _ = outputTests;

        var projectBuilds = await projectBuildsTask.ConfigureAwait(false);
        var sourceDir = Path.Combine(project.Path, "src");
        var sourcePaths = Directory.EnumerateFiles(sourceDir, "*.az", SearchOption.AllDirectories);
        var testSourcePaths = Directory.EnumerateFiles(sourceDir, "*.azt", SearchOption.AllDirectories);
        // Wait for the references, unfortunately, this requires an ugly loop.
        var referenceTasks = project.References.ToDictionaryWithValue(r => projectBuilds[r.Project]);
        var references = new HashSet<PackageReference>();
        foreach (var (reference, packageTask) in referenceTasks)
        {
            var packageResult = await packageTask.ConfigureAwait(false);
            if (packageResult is var (package, _))
                references.Add(new PackageReference(reference.NameOrAlias, package, reference.IsTrusted));
        }

        lock (consoleLock)
        {
            Console.WriteLine($"Compiling {project.Name} ({project.Path})...");
        }
        var codeFiles = LoadCode(sourcePaths, isTest: false);
        var testCodeFiles = LoadCode(testSourcePaths, isTest: true);
        try
        {
            var (package, packageNode) = compiler.CompilePackage(project.Name, codeFiles, testCodeFiles, references);
            // TODO switch to the async version of the compiler
            //var codeFiles = sourcePaths.Select(p => new CodePath(p)).ToList();
            //var references = project.References.ToDictionary(r => r.Name, r => projectBuilds[r.Project]);
            //var package = await compiler.CompilePackageAsync(project.Name, codeFiles, references);

            if (OutputDiagnostics(project, package.Diagnostics, consoleLock))
                return (package, packageNode);

            lock (consoleLock)
            {
                Console.WriteLine($"Compile SUCCEEDED {project.Name} ({project.Path})");
            }

            return (package, packageNode);
        }
        catch (FatalCompilationErrorException ex)
        {
            OutputDiagnostics(project, ex.Diagnostics, consoleLock);
            return null;
        }

        IEnumerable<CodeFile> LoadCode(IEnumerable<string> paths, bool isTest)
            => paths.Select(p => ProjectSet.LoadCode(p, sourceDir, project.RootNamespace, isTest));
    }

    private static CodeFile LoadCode(
        string path,
        string sourceDir,
        IFixedList<string> rootNamespace,
        bool isTest)
    {
        var relativeDirectory = Path.GetDirectoryName(Path.GetRelativePath(sourceDir, path)) ?? throw new InvalidOperationException("Null directory name");
        var ns = rootNamespace.Concat(relativeDirectory.SplitOrEmpty(Path.DirectorySeparatorChar)).ToFixedList();
        return CodeFile.Load(path, ns, isTest);
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

    private static bool OutputDiagnostics(Project project, IFixedList<Diagnostic> diagnostics, object consoleLock)
    {
        if (!diagnostics.Any())
            return false;
        lock (consoleLock)
        {
            Console.WriteLine($@"Build FAILED {project.Name} ({project.Path})");
            foreach (var group in diagnostics.GroupBy(d => d.File))
            {
                var fileDiagnostics = @group.ToList();
                foreach (var diagnostic in fileDiagnostics.Take(10))
                {
                    Console.WriteLine(
                        $"{diagnostic.File.Reference}:{diagnostic.StartPosition.Line}:{diagnostic.StartPosition.Column} {diagnostic.Level} {diagnostic.ErrorCode}");
                    Console.WriteLine("    " + diagnostic.Message);
                }

                if (fileDiagnostics.Count > 10)
                {
                    Console.WriteLine($"{@group.Key.Reference}");
                    Console.WriteLine(
                        $"    {fileDiagnostics.Skip(10).Count(d => d.Level >= DiagnosticLevel.CompilationError)} more errors not shown.");
                    Console.WriteLine(
                        $"    {fileDiagnostics.Skip(10).Count(d => d.Level == DiagnosticLevel.Warning)} more warnings not shown.");
                }
            }
        }

        return true;
    }

    private static string EmitIL(Project project, Package package, bool outputTests, string cacheDir)
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
        var projectAlive = projects.Values.ToDictionary(p => p, _ => SortState.Alive);
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
            case SortState.Dead: // Already visited
                return;

            case SortState.Undead:// Cycle
                throw new Exception("Dependency Cycle");

            case SortState.Alive:
                state[project] = SortState.Undead;
                foreach (var referencedProject in project.References.Select(r => r.Project))
                    TopologicalSortVisit(referencedProject, state, sorted);
                state[project] = SortState.Dead;
                sorted.Add(project);
                return;

            default:
                throw ExhaustiveMatch.Failed(state[project]);
        }
    }

    private enum SortState
    {
        Alive,
        Undead,
        Dead,
    }
}
