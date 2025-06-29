using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Lab.Build;

internal class ProjectFacet
{
    public Project Project { get; }
    public FacetKind Kind { get; }
    public IFixedList<ProjectReference> References { get; }

    public ProjectFacet(Project project, FacetKind kind)
    {
        Project = project;
        Kind = kind;
        var minRelation = Kind == FacetKind.Main ? ProjectRelation.Internal : ProjectRelation.Dev;
        References = project.References.Where(r => r.Relation >= minRelation).ToFixedList();
    }
}
