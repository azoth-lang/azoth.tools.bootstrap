using System.Diagnostics;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;

/// <summary>
/// The context a type constructor is in. This supports nested types by allowing type constructors
/// to exist inside of other type constructors.
/// </summary>
[Closed(
    typeof(BuiltInContext),
    typeof(NamespaceContext),
    typeof(TypeConstructor))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class TypeConstructorContext : IEquatable<TypeConstructorContext>
{
    /// <summary>
    /// Append the prefix the context applies to the name of type constructors in the context to the
    /// given <see cref="StringBuilder"/>.
    /// </summary>
    public abstract void AppendContextPrefix(StringBuilder builder, ConstructedPlainType? containingType);

    #region Equality
    public abstract bool Equals(TypeConstructorContext? other);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is TypeConstructorContext other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
