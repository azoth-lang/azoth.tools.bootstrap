using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class GenericParameterSyntax : CodeSyntax, IGenericParameterSyntax
{
    public ICapabilityConstraintSyntax Constraint { get; }
    public IdentifierName Name { get; }
    public ParameterIndependence Independence { get; }
    public ParameterVariance Variance { get; }

    public GenericParameterSyntax(
        TextSpan span,
        ICapabilityConstraintSyntax constraint,
        IdentifierName name,
        ParameterIndependence independence,
        ParameterVariance variance)
        : base(span)
    {
        Constraint = constraint;
        Name = name;
        Independence = independence;
        Variance = variance;
    }

    public override string ToString()
    {
        return (Independence, Variance) switch
        {
            (ParameterIndependence.None, ParameterVariance.Invariant) => Name.ToString(),
            (ParameterIndependence.None, _) => $"{Name} {Variance.ToSourceCodeString()}",
            (_, ParameterVariance.Invariant) => $"{Name} {Independence.ToSourceCodeString()}",
            _ => $"{Name} {Independence.ToSourceCodeString()} {Variance.ToSourceCodeString()}"
        };
    }
}
