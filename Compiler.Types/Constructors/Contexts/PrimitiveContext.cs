using System.Text;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;

/// <summary>
/// The context for type constructors for simple and literal types. This represents not being in any
/// package or namespace.
/// </summary>
public sealed class PrimitiveContext : TypeConstructorContext
{
    #region Singleton
    public static readonly PrimitiveContext Instance = new PrimitiveContext();

    private PrimitiveContext() { }
    #endregion

    public override void AppendContextPrefix(StringBuilder builder) { /* no prefix */ }

    #region Equality
    public override bool Equals(TypeConstructorContext? other)
        // This type is a singleton, so we can use reference equality.
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(PrimitiveContext));
    #endregion

    public override string ToString() => "⧼primitive-context⧽";
}
