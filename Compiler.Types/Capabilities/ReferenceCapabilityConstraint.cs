namespace Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

public sealed class ReferenceCapabilityConstraint : IReferenceCapabilityConstraint
{
    /// <summary>
    /// Any capability that is directly readable without conversion (i.e. `mut`, `const`, `temp const`, `read`).
    /// </summary>
    ///
    public static readonly ReferenceCapabilityConstraint Readable = new("readable");

    // shareable (i.e. `const, `id`)

    // any (i.e. `iso`, `temp iso`, `mut`, `const`, `temp const`, `read`, `id`)

    // aliasable (default) (`mut`, `const`, `temp const`, `read`, `id`)

    // sendable (i.e. `iso`, `const`, `id`)

    public bool AllowsRead => true;

    public bool AllowsWrite => false;

    public bool AllowsWriteAliases => true;

    private ReferenceCapabilityConstraint(string name)
    {
        this.name = name;
    }

    private readonly string name;

    // TODO is it correct this can't be assigned from `iso` or `temp iso`? May be allowed because it's a subtype of `mut`
    public bool IsAssignableFrom(IReferenceCapabilityConstraint from)
        // i.e. can be read from and isn't `iso` or `temp iso`
        => from.AllowsRead && !(from.AllowsWrite && !from.AllowsWriteAliases);

    public override string ToString() => name;
}
