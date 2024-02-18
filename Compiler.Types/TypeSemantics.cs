using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// The semantics of values of a type
/// </summary>
// TODO it isn't clear this provides any value beyond the move/copy distinction on value types
public enum TypeSemantics
{
    /// <summary>
    /// Types with never value semantics have no values, but they are assignable
    /// to all types. That is, they are a true bottom type.
    /// </summary>
    /// <remarks>
    /// Only <see cref="NeverType"/> and <see cref="UnknownType"/> have this
    /// semantics
    /// </remarks>
    Never = -1,

    /// <summary>
    /// Void value semantics means values of this type can never be used,
    /// assigned to variables, or passed because there are no values of this type.
    /// </summary>
    /// <remarks>
    /// Only <see cref="VoidType"/> has this semantics
    /// </remarks>
    Void = 0,

    /// <summary>
    /// For value types with move semantics, any time the value is used, it is moved
    /// from the source to the destination.
    /// </summary>
    MoveValue = 1,

    /// <summary>
    /// Values of types with copy semantics are copied each time they are used.
    /// </summary>
    /// <remarks>
    /// For <see cref="SimpleType"/>, copy semantics means a bitwise copy.
    /// However, for other value types, copy semantics could mean calling the
    /// copy constructor. Currently, all copy types are <see cref="ValueType"/>.
    /// </remarks>
    CopyValue,

    /// <summary>
    /// Types with reference semantics represent references to values where
    /// more than one reference may exist. When using a reference value,
    /// the reference is copied, but refers to the same object. Reference
    /// capabilities and reachability checks are applied to references
    /// to prevent issues such as dangling references or shared mutability.
    /// </summary>
    /// <remarks>
    /// Variable references may be a distinct category of value semantics
    /// from this.
    /// </remarks>
    Reference,
}
