using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

/// <summary>
/// An equation that contributes a value to an attribute.
/// </summary>
/// <remarks>The attribute being contributed to is often on another node, but doesn't have to be.
/// Since these equations are contributors, multiple equations with the same name should be kept and
/// inherited on nodes.</remarks>
[Closed(
    typeof(InheritedAttributeEquationModel),
    typeof(PreviousAttributeEquationModel),
    typeof(AggregateAttributeEquationModel),
    typeof(CollectionAttributeEquationModel))]
public abstract class ContributorEquationModel : EquationModel
{
    protected ContributorEquationModel(
        AspectModel aspect,
        InternalSymbol nodeSymbol,
        string name,
        bool isMethod,
        string? expression)
        : base(aspect, nodeSymbol, name, isMethod, expression) { }

    protected T ComputeAttributeFamily<T>()
        where T : AttributeFamilyModel
        => Aspect.Tree.AllAttributeFamilies.OfType<T>()
                 .Where(f => f.Name == Name).TrySingle()
           ?? throw new FormatException($"No attribute family for attribute equation {this}");
}
