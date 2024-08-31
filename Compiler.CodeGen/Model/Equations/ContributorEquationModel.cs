using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

/// <summary>
/// An equation that contributes a value to an attribute.
/// </summary>
/// <remarks>The attribute being contributed to is often on another node, but doesn't have to be.
/// Since these equations are contributors, multiple equations with the same name should be kept and
/// inherited on nodes.</remarks>
[Closed(typeof(InheritedAttributeEquationModel), typeof(AggregateAttributeEquationModel))]
public abstract class ContributorEquationModel : EquationModel
{
    protected ContributorEquationModel(
        AspectModel aspect,
        InternalSymbol nodeSymbol,
        string name,
        bool isMethod,
        string? expression)
        : base(aspect, nodeSymbol, name, isMethod, expression) { }
}
