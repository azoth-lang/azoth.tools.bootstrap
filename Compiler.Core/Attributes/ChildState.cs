namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

internal enum ChildState
{
    /// <summary>
    /// No child has been set. This is an invalid state.
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// The initial child value has been set.
    /// </summary>
    Initial,

    /// <summary>
    /// Rewrites are in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// The final child value has been set. No further rewrites are allowed.
    /// </summary>
    Final,
}
