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
}
