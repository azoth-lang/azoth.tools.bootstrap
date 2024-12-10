using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Bare;

[Closed(
    typeof(ConstructedBareType),
    typeof(AssociatedPlainType))]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Using as a trait.")]
// ReSharper disable once InconsistentNaming
public interface BareType
{

}
