using System.Threading;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// The state an attribute is in. That is whether it has been computed, is in the process of being
/// computed, or has not been computed.
/// </summary>
/// <remarks>This must be an <see cref="uint"/> to ensure <see cref="Interlocked"/> can be used on
/// it.</remarks>
public enum AttributeState : uint
{
    Pending = 0,
    InProgress,
    Fulfilled
}
