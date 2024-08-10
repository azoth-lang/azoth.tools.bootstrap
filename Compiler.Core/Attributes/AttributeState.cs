namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// The state an attribute is in. That is whether it has been computed, is in the process of being
/// computed, or has not been computed.
/// </summary>
// TODO Remove: part of old attribute grammar framework
internal enum AttributeState
{
    Pending = 0,
    InProgress,
    Fulfilled
}
