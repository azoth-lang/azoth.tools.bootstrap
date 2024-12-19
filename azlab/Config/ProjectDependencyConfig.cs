using Newtonsoft.Json;

namespace Azoth.Tools.Bootstrap.Lab.Config;

internal class ProjectDependencyConfig
{
    [JsonProperty("path")]
    public string? Path { get; set; }

    [JsonProperty("trusted")]
    public bool? Trusted { get; set; }

    [JsonProperty("relation")]
    public ProjectDependencyRelation Relation { get; set; } = ProjectDependencyRelation.Internal;

    [JsonProperty("bundle")]
    public ProjectDependencyRelation Bundle { get; set; } = ProjectDependencyRelation.None;
}
