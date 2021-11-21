namespace Azoth.Tools.Bootstrap.Compiler.Core
{
    /// <summary>
    /// The semantics of the value of an expression.
    /// </summary>
    /// <remarks>Expression semantics apply the rules of value type move and copy
    /// as well as reference isolation and mutation.</remarks>
    public enum ExpressionSemantics
    {
        /// <summary>
        /// Never returns or has unknown return
        /// </summary>
        Never = -1,
        /// <summary>
        /// Expressions of type `void`, don't produce a value
        /// </summary>
        Void = 0,
        /// <summary>
        /// The value is moved
        /// </summary>
        MoveValue = 1,
        /// <summary>
        /// The value is copied. For expression, does not indicate whether it is
        /// safe to bit copy the type or a copy function is needed
        /// </summary>
        CopyValue,
        /// <summary>
        /// Produce an isolated reference to the object. The source reference is converted to `id`
        /// </summary>
        IsolatedReference,
        /// <summary>
        /// Produce a mutable reference to the object
        /// </summary>
        MutableReference,
        /// <summary>
        /// Produce a read only reference to the object
        /// </summary>
        ReadOnlyReference,
        /// <summary>
        /// Take a reference to a place. Used for LValues
        /// </summary>
        CreateReference,
    }
}
