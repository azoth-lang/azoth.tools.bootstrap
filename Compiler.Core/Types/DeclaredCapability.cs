namespace Azoth.Tools.Bootstrap.Compiler.Core.Types;

/// <summary>
/// The reference capability a type was declared with.
/// </summary>
/// <remarks>This is distinct from <c>Capability</c> because that type supports additional
/// capabilities that cannot be directly declared. Also, this type distinguishes
/// <see cref="Default"/>.</remarks>
public enum DeclaredCapability
{
    Isolated = 1,
    TemporarilyIsolated,
    Mutable,
    Default, // read-only or const depending on how the type is declared
    Read, // read-only from this reference, possibly writable from others
    Constant,
    TemporarilyConstant,
    Identity
}
