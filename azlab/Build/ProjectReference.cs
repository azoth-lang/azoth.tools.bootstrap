namespace Azoth.Tools.Bootstrap.Lab.Build;

internal class ProjectReference
{
    public string NameOrAlias { get; }
    public Project Project { get; }
    public bool IsTrusted { get; }

    public ProjectReference(string nameOrAlias, Project project, bool isTrusted)
    {
        NameOrAlias = nameOrAlias;
        Project = project;
        IsTrusted = isTrusted;
    }
}
