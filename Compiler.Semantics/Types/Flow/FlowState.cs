using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

/// <summary>
/// Wraps up all the state that changes with the flow of the code to make it easy to attach to each
/// node in the semantic tree.
/// </summary>
public sealed class FlowState
{
    public static readonly FlowState Empty = new FlowState();

    private FlowState() { }

    public FlowState Alias(ValueId valueToAlias, ValueId alias)
        => throw new System.NotImplementedException();

    /// <summary>
    /// Get the current type of the given value as determined by flow typing.
    /// </summary>
    public DataType Type(ValueId value) => throw new System.NotImplementedException();
}
