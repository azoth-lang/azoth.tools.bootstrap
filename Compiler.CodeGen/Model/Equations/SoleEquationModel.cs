using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

/// <summary>
/// An equation that should be the sole provider of a value for an attribute.
/// </summary>
/// <remarks>These equations are distinct by name (and parameters) per node and as such, there
/// should only be one of these on a given node.</remarks>
[Closed(typeof(SynthesizedAttributeEquationModel), typeof(IntertypeMethodEquationModel))]
public abstract class SoleEquationModel : EquationModel
{
    public static IEqualityComparer<SoleEquationModel> NameAndParametersComparer { get; }
        = EqualityComparer<SoleEquationModel>.Create((a1, a2) => a1?.Name == a2?.Name && a1?.Parameters == a2?.Parameters,
            a => HashCode.Combine(a.Name, a.Parameters));

    public abstract override AttributeModel Attribute { get; }
    public abstract EvaluationStrategy Strategy { get; }
    public abstract string? Parameters { get; }
    public abstract bool RequiresEmitOnNode { get; }

    protected SoleEquationModel(
        AspectModel aspect,
        InternalSymbol nodeSymbol,
        string name,
        bool isMethod,
        string? expression)
        : base(aspect, nodeSymbol, name, isMethod, expression) { }
}
