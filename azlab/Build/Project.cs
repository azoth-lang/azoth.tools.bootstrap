using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Lab.Config;

namespace Azoth.Tools.Bootstrap.Lab.Build;

[DebuggerDisplay($"Project: {{{nameof(Name)},nq}}")]
internal class Project
{
    public string Path { get; }
    public IFixedList<string> RootNamespace { get; }
    public string Name { get; }
    public IFixedList<string> Authors { get; }
    public ProjectTemplate Template { get; }
    public IFixedList<ProjectReference> References { get; }
    public IFixedList<ProjectReference> DevReferences => devReferences.Value;
    private readonly Lazy<IFixedList<ProjectReference>> devReferences;

    public Project(
        ProjectConfig file,
        IEnumerable<ProjectReference> references,
        Lazy<IFixedList<ProjectReference>> devReferences)
    {
        Path = System.IO.Path.GetDirectoryName(file.FullPath) ?? throw new InvalidOperationException("Null directory name");
        Name = file.Name ?? throw new InvalidOperationException();
        RootNamespace = (file.RootNamespace ?? "").SplitOrEmpty('.');
        Authors = (file.Authors ?? throw new InvalidOperationException())
                  .Select(a => a ?? throw new InvalidOperationException()).ToFixedList();
        Template = file.Template ?? throw new InvalidOperationException();
        References = references.ToFixedList();
        this.devReferences = devReferences;
    }
}
