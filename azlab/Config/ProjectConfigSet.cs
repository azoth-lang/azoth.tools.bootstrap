using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Azoth.Tools.Bootstrap.Lab.Config;

/// <summary>
/// A set of project configs. Note that this assumes the name used to refer to a project
/// matches the name in the project.
/// </summary>
internal class ProjectConfigSet : IEnumerable<ProjectConfig>
{
    private readonly Dictionary<string, ProjectConfig> configs = new Dictionary<string, ProjectConfig>();

    public ProjectConfig Load(string packagePath)
    {
        var config = ProjectConfig.Load(packagePath);
        if (configs.TryGetValue(config.Name ?? throw new InvalidOperationException(), out var existingConfig))
            return existingConfig;

        configs.Add(config.Name ?? throw new InvalidOperationException(), config);
        var configDir = Path.GetDirectoryName(config.FullPath!)!;
        foreach (var dependency in config.Dependencies ?? throw new InvalidOperationException())
        {
            var dependencyPath = dependency.Value?.Path ?? throw new InvalidOperationException();
            Load(Path.Combine(configDir, dependencyPath));
        }

        return config;
    }

    public ProjectConfig this[string name] => configs[name];

    public IEnumerator<ProjectConfig> GetEnumerator() => configs.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
