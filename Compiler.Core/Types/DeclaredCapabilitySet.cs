namespace Azoth.Tools.Bootstrap.Compiler.Core.Types;

/// <summary>
/// The capability set a type is declared with.
/// </summary>
public enum DeclaredCapabilitySet
{
    Readable = 1,
    Shareable,
    Aliasable,
    Sendable,
    ReadOnly,
    Temporary,
    Any,
}
