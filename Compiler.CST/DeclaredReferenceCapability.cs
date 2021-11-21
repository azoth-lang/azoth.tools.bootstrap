namespace Azoth.Tools.Bootstrap.Compiler.CST
{
    /// <summary>
    /// The reference capability a type was declared with.
    /// </summary>
    public enum DeclaredReferenceCapability
    {
        Isolated = 1,
        Mutable,
        ReadOnly, // read-only from this reference, possibly writable from others
        Constant,
        Identity,
    }
}
