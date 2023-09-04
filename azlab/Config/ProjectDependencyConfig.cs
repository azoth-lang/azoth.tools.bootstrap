using Newtonsoft.Json;

namespace Azoth.Tools.Bootstrap.Lab.Config;

public class ProjectDependencyConfig
{
    [JsonProperty("path")]
    public string? Path { get; set; }

    [JsonProperty("trusted")]
    public bool? Trusted { get; set; }
}
