using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
    public IFixedList<ProjectReference> References => references ?? [];
    private IFixedList<ProjectReference>? references;

    public Project(ProjectConfig file)
    {
        Path = System.IO.Path.GetDirectoryName(file.FullPath) ?? throw new InvalidOperationException("Null directory name");
        Name = file.Name ?? throw new InvalidOperationException();
        RootNamespace = (file.RootNamespace ?? "").SplitOrEmpty('.');
        Authors = (file.Authors ?? throw new InvalidOperationException())
                  .Select(a => a ?? throw new InvalidOperationException()).ToFixedList();
        Template = file.Template ?? throw new InvalidOperationException();
    }

    internal void AttachReferences(IEnumerable<ProjectReference> references)
    {
        var oldValue = Interlocked.CompareExchange(ref this.references, references.ToFixedList(), null);
        if (oldValue != null) throw new InvalidOperationException("Cannot attach references more than once.");
    }
}
