using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;

/// <summary>
/// The context of a type constructor that is declared in a namespace in a package.
/// </summary>
public sealed class NamespaceContext : TypeConstructorContext
{
    public IdentifierName Package { get; }
    public NamespaceName Namespace { get; }

    /// <remarks>The prefix includes a trailing dot so that the type constructor doesn't need to
    /// check the context type to determine if a dot separator is needed.</remarks>
    public override void AppendContextPrefix(StringBuilder builder, ConstructedPlainType? containingType)
    {
        Requires.Null(containingType, nameof(containingType), "No containing type for namespace context.");
        builder.Append(Package);
        builder.Append("::");
        if (Namespace != NamespaceName.Global)
        {
            builder.Append('.');
            builder.Append(Namespace);
        }
        builder.Append('.');
    }

    public NamespaceContext(IdentifierName package, NamespaceName ns)
    {
        Package = package;
        Namespace = ns;
    }

    #region Equality
    public override bool Equals(TypeConstructorContext? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is NamespaceContext that
            && Package.Equals(that.Package)
            && Namespace.Equals(that.Namespace);
    }

    public override int GetHashCode() => HashCode.Combine(Package, Namespace);
    #endregion

    public override string ToString() => $"{Package}::.{Namespace}";
}
