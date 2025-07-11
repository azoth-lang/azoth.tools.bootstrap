using System.Diagnostics;
using Azoth.Tools.Bootstrap.Lab.Build;
using Newtonsoft.Json;

namespace Azoth.Tools.Bootstrap.Lab.Config;

[DebuggerDisplay($"ProjectDependencyConfig: {{{nameof(Path)}}}")]
internal class ProjectDependencyConfig
{
    [JsonProperty("path")]
    public string? Path { get; set; }

    [JsonProperty("trusted")]
    public bool IsTrusted { get; set; } = false;

    [JsonProperty("relation")]
    public ProjectRelation Relation { get; set; } = ProjectRelation.Internal;

    [JsonProperty("bundle")]
    public ProjectRelation Bundle { get; set; } = ProjectRelation.None;

    [JsonProperty("tests")]
    public bool ReferenceTests { get; set; } = false;
}
