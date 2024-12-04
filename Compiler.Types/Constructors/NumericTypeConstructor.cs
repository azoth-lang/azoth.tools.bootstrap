using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Constructors;

[Closed(typeof(IntegerTypeConstructor), typeof(IntegerLiteralTypeConstructor))]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Using as a trait.")]
// ReSharper disable once InconsistentNaming
public interface NumericTypeConstructor : TypeConstructor
{
    // TODO IntegerLiteralTypeConstructor shouldn't really have a plain type
    ConstructedPlainType PlainType { get; }
}
