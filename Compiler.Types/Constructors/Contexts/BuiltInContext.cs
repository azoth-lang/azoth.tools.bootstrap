using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;

/// <summary>
/// The context for type constructors for built-in types. This represents not being in any
/// package or namespace.
/// </summary>
public sealed class BuiltInContext : BareTypeConstructorContext
{
    #region Singleton
    public static readonly BuiltInContext Instance = new BuiltInContext();

    private BuiltInContext() { }
    #endregion

    public override void AppendContextPrefix(StringBuilder builder, BarePlainType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), "No containing type for built-in context.");
        // no prefix to append
    }

    #region Equality
    public override bool Equals(BareTypeConstructorContext? other)
        // This type is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(BuiltInContext));
    #endregion

    public override string ToString() => "⧼built-in-context⧽";
}
