using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Lab.Build;
using Azoth.Tools.Bootstrap.Lab.Config;
using McMaster.Extensions.CommandLineUtils;

namespace Azoth.Tools.Bootstrap.Lab;

/// <summary>
/// The azoth build and package tool
/// </summary>
public static class Program
{
#if DEBUG
    public const bool DefaultAllowParallel = false;
#else
        public const bool DefaultAllowParallel = true;
#endif

    public static int Main(string[] args)
    {
        using var app = new CommandLineApplication()
        {
            Name = "azlab",
            Description = "Azoth's package manager and build tool"
        };

        app.HelpOption();

        var allowParallelOption = app.Option<bool>("--allow-parallel", "Allow parallel processing", CommandOptionType.SingleOrNoValue, true);
        var maxConcurrencyOption = app.Option<int>("--max-concurrency",
            "The max number of tasks to run concurrently",
            CommandOptionType.SingleValue, true);


        app.Command("build", cmd =>
        {
            cmd.Description = "Compile a package and all of its dependencies to IL";
            var packageOption = cmd.Option("-p|--package <Package-Path>",
                "Package to build", CommandOptionType.SingleValue);
            var verboseOption = cmd.Option("-v|--verbose", "Use verbose output",
                CommandOptionType.NoValue);
            cmd.OnExecuteAsync(async (ct) =>
            {
                var allowParallel = allowParallelOption.OptionalValue() ?? DefaultAllowParallel;
                var maxConcurrency = maxConcurrencyOption.OptionalValue();
                var verbose = verboseOption.HasValue();
                var packagePath = packageOption.Value() ?? ".";

                await BuildAsync(packagePath, verbose, allowParallel, maxConcurrency).ConfigureAwait(false);
            });
        });

        app.Command("interpret", cmd =>
        {
            cmd.Description = "Interpret a package and all of its dependencies";
            var packageOption = cmd.Option("-p|--package <Package-Path>", "Package to interpret",
                CommandOptionType.SingleValue);
            var verboseOption = cmd.Option("-v|--verbose", "Use verbose output", CommandOptionType.NoValue);
            cmd.OnExecuteAsync(async (ct) =>
            {
                var allowParallel = allowParallelOption.OptionalValue() ?? DefaultAllowParallel;
                var maxConcurrency = maxConcurrencyOption.OptionalValue();
                var verbose = verboseOption.HasValue();
                var packagePath = packageOption.Value() ?? ".";

                await InterpretAsync(packagePath, verbose, allowParallel, maxConcurrency).ConfigureAwait(false);
            });
        });

        app.Command("test", cmd =>
        {
            cmd.Description = "Run tests in a package";
            var packageOption = cmd.Option("-p|--package <Package-Path>", "Package to test",
                CommandOptionType.SingleValue);
            var verboseOption = cmd.Option("-v|--verbose", "Use verbose output", CommandOptionType.NoValue);
            cmd.OnExecuteAsync(async (ct) =>
            {
                var allowParallel = allowParallelOption.OptionalValue() ?? DefaultAllowParallel;
                var maxConcurrency = maxConcurrencyOption.OptionalValue();
                var verbose = verboseOption.HasValue();
                var packagePath = packageOption.Value() ?? ".";

                await TestAsync(packagePath, verbose, allowParallel, maxConcurrency).ConfigureAwait(false);
            });
        });

        app.OnExecute(() =>
        {
            app.ShowHelp();
            return 1;
        });

        return app.Execute(args);
    }

    private static Task BuildAsync(
        string packagePath,
        bool verbose,
        bool allowParallel,
        int? maxConcurrency)
    {
        var configs = new ProjectConfigSet();
        configs.Load(packagePath);
        var projectSet = new ProjectSet();
        projectSet.AddAll(configs);
        var taskScheduler = NewTaskScheduler(
            allowParallel,
            maxConcurrency);
        return projectSet.BuildAsync(taskScheduler, verbose);
    }

    private static Task InterpretAsync(
        string packagePath,
        bool verbose,
        bool allowParallel,
        int? maxConcurrency)
    {
        var configs = new ProjectConfigSet();
        var projectConfig = configs.Load(packagePath);
        var projectSet = new ProjectSet();
        projectSet.AddAll(configs);
        var taskScheduler = NewTaskScheduler(allowParallel, maxConcurrency);
        return projectSet.InterpretAsync(taskScheduler, verbose, projectConfig);
    }

    private static Task TestAsync(
        string packagePath,
        bool verbose,
        bool allowParallel,
        int? maxConcurrency)
    {
        var configs = new ProjectConfigSet();
        var projectConfig = configs.Load(packagePath);
        var projectSet = new ProjectSet();
        projectSet.AddAll(configs);
        var taskScheduler = NewTaskScheduler(allowParallel, maxConcurrency);
        return projectSet.TestAsync(taskScheduler, verbose, projectConfig);
    }

    private static TaskScheduler NewTaskScheduler(bool allowParallel, int? maxConcurrency)
    {
        ConcurrentExclusiveSchedulerPair taskSchedulerPair;
        if (maxConcurrency is int concurrency)
            taskSchedulerPair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, concurrency);
        else
            taskSchedulerPair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default);

        return allowParallel
            ? taskSchedulerPair.ConcurrentScheduler
            : taskSchedulerPair.ExclusiveScheduler;
    }
}
