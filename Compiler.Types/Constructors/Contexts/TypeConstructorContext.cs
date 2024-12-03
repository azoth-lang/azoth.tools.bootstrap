using System.Diagnostics.CodeAnalysis;
using System.Text;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors.Contexts;

[Closed(
    typeof(PrimitiveContext),
    typeof(NamespaceContext),
    typeof(TypeConstructor))]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Using as a trait.")]
// ReSharper disable once InconsistentNaming
public interface TypeConstructorContext : IEquatable<TypeConstructorContext>
{
    /// <summary>
    /// Append the prefix the context applies to the name of type constructors in the context to the
    /// given <see cref="StringBuilder"/>.
    /// </summary>
    void AppendContextPrefix(StringBuilder builder);

    string ToString();
}
