namespace Azoth.Tools.Bootstrap.Lab.Build;

internal class ProjectReference
{
    public string Alias { get; }
    public Project Project { get; }
    public bool IsTrusted { get; }
    public ProjectRelation Relation { get; }
    public ProjectRelation Bundle { get; }

    public ProjectReference(string alias, Project project, bool isTrusted, ProjectRelation relation, ProjectRelation bundle)
    {
        Alias = alias;
        Project = project;
        IsTrusted = isTrusted;
        Relation = relation;
        Bundle = bundle;
    }
}
