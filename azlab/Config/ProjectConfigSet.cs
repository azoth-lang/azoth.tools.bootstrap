using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Azoth.Tools.Bootstrap.Lab.Config;

/// <summary>
/// A set of project configs.
/// </summary>
/// <remarks>This organizes project configs by their project name which may not match the alias used
/// for them as a dependency.</remarks>
internal class ProjectConfigSet : IEnumerable<ProjectConfig>
{
    private readonly Dictionary<string, ProjectConfig> configs = new();
    private readonly Dictionary<ProjectConfig, Dictionary<string, ProjectConfig>> configReferences = new();

    public ProjectConfig Load(string packagePath)
    {
        var config = ProjectConfig.Load(packagePath);
        if (configs.TryGetValue(config.Name ?? throw new InvalidOperationException(), out var existingConfig))
            return existingConfig;

        configs.Add(config.Name, config);
        var references = new Dictionary<string, ProjectConfig>();
        configReferences.Add(config, references);
        var configDir = Path.GetDirectoryName(config.FullPath!)!;
        foreach (var (alias, dependencyConfig) in config.Dependencies ?? throw new InvalidOperationException())
        {
            var dependencyPath = dependencyConfig?.Path ?? throw new InvalidOperationException();
            var referencedProjectConfig = Load(Path.Combine(configDir, dependencyPath));
            references.Add(alias, referencedProjectConfig);
        }

        return config;
    }

    public ProjectConfig this[string name] => configs[name];

    public ProjectConfig this[ProjectConfig referencingProject, string dependencyAlias]
        => configReferences[referencingProject][dependencyAlias];

    public IEnumerator<ProjectConfig> GetEnumerator() => configs.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
