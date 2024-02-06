namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

public sealed class ReferenceCapabilityConstraint : IReferenceCapabilityConstraint
{
    public static readonly ReferenceCapabilityConstraint Readable = new("readable");

    public bool AllowsRead => true;

    public bool AllowsWrite => false;

    private ReferenceCapabilityConstraint(string name)
    {
        this.name = name;
    }

    private readonly string name;

    public bool IsAssignableFrom(IReferenceCapabilityConstraint from)
        => from.AllowsRead;

    public override string ToString() => name;
}
