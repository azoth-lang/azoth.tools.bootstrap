using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;

namespace Azoth.Tools.Bootstrap.Lab.Config;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes",
    Justification = "Instantiated by JSON converter")]
[DebuggerDisplay($"ProjectConfig: {{{nameof(Name)},nq}}")]
internal class ProjectConfig
{
    public const string FileName = "azlab-project.vson";

    [JsonIgnore]
    public string? FullPath { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("root_namespace")]
    public string? RootNamespace { get; set; }

    [JsonProperty("authors")]
    public List<string?>? Authors { get; set; }

    [JsonProperty("template")]
    public ProjectTemplate? Template { get; set; }

    [JsonProperty("dependencies")]
    public Dictionary<string, ProjectDependencyConfig?>? Dependencies { get; set; } = new Dictionary<string, ProjectDependencyConfig?>();

    public static ProjectConfig Load(string path)
    {
        var extension = Path.GetExtension(path);
        string projectFilePath;
        if (Directory.Exists(path))
            projectFilePath = Path.Combine(path, FileName);
        else if (extension == "vson")
            projectFilePath = path;
        else
            throw new Exception($"Unexpected project file extension '.{extension}'");

        projectFilePath = Path.GetFullPath(projectFilePath);

        using var file = new JsonTextReader(File.OpenText(projectFilePath));
        var serializer = new JsonSerializer();
        var projectFile = serializer.Deserialize<ProjectConfig>(file) ?? throw new NullReferenceException();
        projectFile.FullPath = projectFilePath;
        return projectFile;
    }
}
