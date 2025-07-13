using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.API;
using Azoth.Tools.Bootstrap.Compiler.Core;
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
    private readonly Dictionary<(Project, FacetKind), ProjectFacet> projectFacets = [];

    public void AddAll(ProjectConfigSet configs)
    {
        foreach (var config in configs)
            GetOrAddProject(config, configs);
    }

    private Project GetProject(ProjectConfig config)
    {
        var projectDir = GetDirectoryName(config);
        return projects[projectDir];
    }

    private ProjectFacet GetProjectFacet(Project project, FacetKind facet)
        => projectFacets[(project, facet)];

    private Project GetOrAddProject(ProjectConfig config, ProjectConfigSet configs)
    {
        var projectDir = GetDirectoryName(config);
        if (projects.TryGetValue(projectDir, out var existingProject))
            return existingProject;

        var project = new Project(config);
        projects.Add(projectDir, project);
        project.AttachReferences(CreateReferences(config));
        // Create facets
        AddFacet(project, FacetKind.Main);
        AddFacet(project, FacetKind.Tests);
        return project;

        IEnumerable<ProjectReference> CreateReferences(ProjectConfig projectConfig)
            // TODO be more exact about selecting distinct references or possibly even merging them
            => projectConfig.Dependencies!
                .SelectMany((s, dependencyConfig) => CreateReferencesForDependency(projectConfig, s, dependencyConfig))
                .DistinctBy(r => r.Project.Name);

        IEnumerable<ProjectReference> CreateReferencesForDependency(
            ProjectConfig projectConfig,
            string alias,
            ProjectDependencyConfig? dependencyConfig)
        {
            // Note: these are input validations
            // TODO do these validations earlier
            if (dependencyConfig is null)
                throw new InvalidOperationException("Dependency must not be null.");
            if (dependencyConfig.Relation == ProjectRelation.None)
                throw new InvalidOperationException("None is not a valid relation.");
            var dependencyProjectConfig = configs[config, alias];
            if (dependencyProjectConfig == projectConfig)
                // TODO this is an input validation, it should probably be done earlier
                throw new InvalidOperationException("Project cannot reference itself.");
            var dependencyProject = GetOrAddProject(dependencyProjectConfig, configs);
            var isTrusted = dependencyConfig.IsTrusted;
            yield return new(alias, dependencyProject, isTrusted, dependencyConfig.Relation, dependencyConfig.Bundle, dependencyConfig.ReferenceTests);
            // When bundling don't bundle in a reference to the current project
            foreach (var bundledReference in dependencyProject.References.Where(r => r.Bundle != ProjectRelation.None && r.Project.Name != projectConfig.Name))
            {
                // Relation is the min because if this is a dev reference so is the bundled thing
                var relation = dependencyConfig.Relation.Min(bundledReference.Bundle);
                const ProjectRelation bundle = ProjectRelation.None; // bundling is non-recursive
                const bool referenceTests = false; // Bundling doesn't apply on tests
                yield return new(bundledReference.Project.Name, bundledReference.Project,
                    isTrusted && bundledReference.IsTrusted, relation, bundle, referenceTests);
            }
        }
    }

    private void AddFacet(Project project, FacetKind facet)
    {
        var projectFacet = new ProjectFacet(project, facet);
        projectFacets.Add((project, facet), projectFacet);
    }

    private static string GetDirectoryName(ProjectConfig config)
        => Path.GetDirectoryName(config.FullPath) ?? throw new InvalidOperationException("Null directory name");

    public IEnumerator<Project> GetEnumerator() => projects.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task BuildAsync(TaskScheduler taskScheduler, bool verbose)
    {
        try
        {
            await ProcessProjects(taskScheduler, verbose, BuildAsync);
        }
        catch (FatalCompilationErrorException)
        {
            // Errors already output, nothing to do
        }
    }

    private delegate Task<IPackageFacetNode> ProcessAsync(
        AzothCompiler compiler,
        ProjectFacet projectFacet,
        Task<FixedDictionary<ProjectFacet, Task<IPackageFacetNode>>> projectBuildsTask,
        AsyncLock consoleLock);

    private async Task<IFixedSet<IPackageFacetNode>> ProcessProjects(TaskScheduler taskScheduler, bool verbose, ProcessAsync processAsync)
    {
        _ = verbose; // verbose parameter will be needed in the future
        var taskFactory = new TaskFactory(taskScheduler);
        var projectFacetBuilds = new Dictionary<ProjectFacet, Task<IPackageFacetNode>>();

        var projectBuildsSource = new TaskCompletionSource<FixedDictionary<ProjectFacet, Task<IPackageFacetNode>>>();
        var projectBuildsTask = projectBuildsSource.Task;

        // Sort projects to detect cycles, and so we can assume the tasks already exist
        var sortedProjectFacets = TopologicalSortFacets();
        var compiler = new AzothCompiler();
        var consoleLock = new AsyncLock();
        foreach (var projectFacet in sortedProjectFacets)
        {
            var buildTask = taskFactory.StartNew(() => processAsync(compiler, projectFacet, projectBuildsTask, consoleLock))
                                       .Unwrap(); // Needed because StartNew doesn't work intuitively with Async methods
            if (!projectFacetBuilds.TryAdd(projectFacet, buildTask))
                throw new Exception("Project added to build set twice");
        }

        projectBuildsSource.SetResult(projectFacetBuilds.ToFixedDictionary());

        var allBuilds = await Task.WhenAll(projectFacetBuilds.Values).ConfigureAwait(false);
        return allBuilds.ToFixedSet();
    }

    private (IPackageFacetNode, IFixedSet<IPackageFacetNode>) FindEntryFacet(
        IFixedSet<IPackageFacetNode> facets,
        ProjectConfig entryProjectConfig,
        FacetKind entryFacetKind)
    {
        var entryProjectFacet = GetProjectFacet(GetProject(entryProjectConfig), entryFacetKind);
        var entryFacet = facets.Single(f => f.PackageName == entryProjectFacet.Project.Name
                                            && f.Kind == entryProjectFacet.Kind);
        var referencedPackages = facets.WhereNotNull().Except(entryFacet).ToFixedSet();
        return (entryFacet, referencedPackages);
    }

    public async Task InterpretAsync(TaskScheduler taskScheduler, bool verbose, ProjectConfig entryProjectConfig)
    {
        try
        {
            var facets = await ProcessProjects(taskScheduler, verbose, AnalyzeAsync);
            var (entryFacet, referencedFacets) = FindEntryFacet(facets, entryProjectConfig, FacetKind.Main);
            var interpreter = new AzothTreeInterpreter();
            Console.WriteLine($"Running {entryFacet.PackageName}...");
            var process = interpreter.Execute(entryFacet, referencedFacets);
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
            var facets = await ProcessProjects(taskScheduler, verbose, AnalyzeAsync);
            var (testFacet, referencedFacets) = FindEntryFacet(facets, testProjectConfig, FacetKind.Tests);
            var interpreter = new AzothTreeInterpreter();
            var process = interpreter.ExecuteTests(testFacet, referencedFacets);
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

    private static async Task<IPackageFacetNode> BuildAsync(
        AzothCompiler compiler,
        ProjectFacet projectFacet,
        Task<FixedDictionary<ProjectFacet, Task<IPackageFacetNode>>> projectBuildsTask,
        AsyncLock consoleLock)
    {
        var packageFacet = await AnalyzeAsync(compiler, projectFacet, projectBuildsTask, consoleLock);
        var project = projectFacet.Project;
        var cacheDir = PrepareCacheDir(project);
        var codePath = EmitIL(projectFacet, packageFacet, cacheDir);

        using (await consoleLock.LockAsync())
        {
            Console.WriteLine($"Build SUCCEEDED {project.Name} {projectFacet.Kind} ({project.Path})");
        }

        return packageFacet;
    }

    private static async Task<IPackageFacetNode> AnalyzeAsync(
        AzothCompiler compiler,
        ProjectFacet projectFacet,
        Task<FixedDictionary<ProjectFacet, Task<IPackageFacetNode>>> projectFacetBuildsTask,
        AsyncLock consoleLock)
    {
        var project = projectFacet.Project;
        var projectFacetBuilds = await projectFacetBuildsTask.ConfigureAwait(false);
        using (await consoleLock.LockAsync())
        {
            Console.WriteLine($"Compiling {project.Name} {projectFacet.Kind} ({project.Path}) ...");
        }
        var sourceDir = Path.Combine(project.Path, "src");
        var symbolLoader = CreateSymbolLoader(projectFacetBuilds);
        var references = projectFacet.References.Select(r => r.ToPackageReference()).WhereNotNull().ToList();
        var isTest = projectFacet.Kind == FacetKind.Tests;
        var fileExtension = isTest ? "*.azt" : "*.az";
        var sourcePaths = Directory.EnumerateFiles(sourceDir, fileExtension, SearchOption.AllDirectories);
        var codeFiles = CreateCodePaths(sourcePaths, isTest);
        try
        {
            var facet = await compiler.AnalyzePackageFacetAsync(project.Name, projectFacet.Kind, codeFiles, references, symbolLoader);

            if (OutputDiagnostics(projectFacet, facet.Diagnostics, consoleLock))
                return facet;

            using (await consoleLock.LockAsync())
            {
                Console.WriteLine($"Compile SUCCEEDED {project.Name} {projectFacet.Kind} ({project.Path})");
            }

            return facet;
        }
        catch (FatalCompilationErrorException ex)
        {
            OutputDiagnostics(projectFacet, ex.Diagnostics, consoleLock);
            throw;
        }

        static PackageSymbolLoader CreateSymbolLoader(FixedDictionary<ProjectFacet, Task<IPackageFacetNode>> projectBuilds)
        {
            var entries = projectBuilds.Select((facet, task)
                => KeyValuePair.Create(((IdentifierName)facet.Project.Name, Facet: facet.Kind), task));
            return new(entries);
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

    private static bool OutputDiagnostics(ProjectFacet projectFacet, DiagnosticCollection diagnostics, AsyncLock consoleLock)
    {
        if (diagnostics.IsEmpty)
            return false;
        var project = projectFacet.Project;
        using (consoleLock.Lock())
        {
            Console.WriteLine($"Build FAILED {project.Name} {projectFacet.Kind} ({project.Path})");
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

    private static string EmitIL(ProjectFacet project, IPackageFacetNode package, string cacheDir)
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

    private List<ProjectFacet> TopologicalSortFacets()
    {
        var states = projectFacets.Values.ToDictionary(p => p, _ => SortState.Unvisited);
        var sorted = new List<ProjectFacet>(projects.Count);
        foreach (var projectFacet in projectFacets.Values)
            Visit(projectFacet);

        return sorted;

        void Visit(ProjectFacet projectFacet)
        {
            switch (states[projectFacet])
            {
                case SortState.Visited:
                    return;

                case SortState.Visiting: // Cycle
                    throw new Exception("Dependency Cycle");

                case SortState.Unvisited:
                    states[projectFacet] = SortState.Visiting;
                    if (projectFacet.Kind == FacetKind.Tests)
                    {
                        // Visit other test projects first to push them earlier/closer to the main their main facet
                        VisitReferences(projectFacet, FacetKind.Tests);
                        Visit(GetProjectFacet(projectFacet.Project, FacetKind.Main));
                    }
                    VisitReferences(projectFacet, FacetKind.Main);
                    states[projectFacet] = SortState.Visited;
                    sorted.Add(projectFacet);
                    return;

                default:
                    throw ExhaustiveMatch.Failed(states[projectFacet]);
            }
        }

        void VisitReferences(ProjectFacet projectFacet, FacetKind facet)
        {
            var references = projectFacet.References.AsEnumerable();
            if (facet == FacetKind.Tests)
                references = references.Where(r => r.ReferenceTests);
            foreach (var referencedFacet in references.Select(r => GetProjectFacet(r.Project, facet)))
                Visit(referencedFacet);
        }
    }

    private enum SortState
    {
        Unvisited,
        Visiting,
        Visited,
    }
}
