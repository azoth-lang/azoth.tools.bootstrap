using System.Diagnostics;
using System.IO;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Lab.Build;

[DebuggerDisplay($"ProjectReference: {{{nameof(Alias)},nq}} = {{{nameof(Path)}}}")]
internal class ProjectReference
{
    public string Alias { get; }
    public Project Project { get; }
    public bool IsTrusted { get; }
    public ProjectRelation Relation { get; }
    public ProjectRelation Bundle { get; }

    public ProjectReference(string alias, Project project, bool isTrusted, ProjectRelation relation, ProjectRelation bundle)
    {
        Requires.NotNull(project, nameof(project));
        Alias = alias;
        Project = project;
        IsTrusted = isTrusted;
        Relation = relation;
        Bundle = bundle;
    }
}
