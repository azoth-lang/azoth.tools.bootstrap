namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

public enum ControlFlowKind
{
    /// <summary>
    /// A regular control flow link (i.e. not a loop).
    /// </summary>
    /// <remarks>In forward flow, these are forward links. But in backward flow, these are backward links.</remarks>
    Normal = 1,

    /// <summary>
    /// A control flow link that represents a loop.
    /// </summary>
    /// <remarks>In forward flow, these are back links. But in backward flow, these are forward links.</remarks>
    Loop = 2,
}
