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
    typeof(BareTypeConstructor))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class BareTypeConstructorContext : IEquatable<BareTypeConstructorContext>
{
    /// <summary>
    /// Append the prefix the context applies to the name of type constructors in the context to the
    /// given <see cref="StringBuilder"/>.
    /// </summary>
    public abstract void AppendContextPrefix(StringBuilder builder, BarePlainType? containingType);

    #region Equality
    public abstract bool Equals(BareTypeConstructorContext? other);

    public sealed override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is BareTypeConstructorContext other && Equals(other);

    public abstract override int GetHashCode();
    #endregion

    public abstract override string ToString();
}
