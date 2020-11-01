namespace Azoth.Tools.Bootstrap.Compiler.CST
{
    /// <summary>
    /// The reference capability a type was declared with. In addition to the standard
    /// reference capabilities, this also adds 'Mutable' and 'Readable' which represent
    /// `mut T` and `T` respectively. This is because a declared type may not fully
    /// determine the reference capability. For these two cases, whether the type is
    /// shared or lent must be inferred.
    /// </summary>
    public enum DeclaredReferenceCapability
    {
        Isolated = 1,
        LentIsolated,
        Transition,
        LentTransition,
        Mutable,
        SharedMutable,
        LentMutable,
        Const,
        LentConst,
        Readable,
        Shared,
        Lent,
        Identity
    }
}
