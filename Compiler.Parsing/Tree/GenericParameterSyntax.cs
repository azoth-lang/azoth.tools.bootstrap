using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class GenericParameterSyntax : CodeSyntax, IGenericParameterSyntax
{
    public ICapabilityConstraintSyntax Constraint { get; }
    public IdentifierName Name { get; }
    public TypeParameterIndependence Independence { get; }
    public ParameterVariance Variance { get; }

    public GenericParameterSyntax(
        TextSpan span,
        ICapabilityConstraintSyntax constraint,
        IdentifierName name,
        TypeParameterIndependence independence,
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
            (TypeParameterIndependence.None, ParameterVariance.Invariant) => Name.ToString(),
            (TypeParameterIndependence.None, _) => $"{Name} {Variance.ToSourceCodeString()}",
            (_, ParameterVariance.Invariant) => $"{Name} {Independence.ToSourceCodeString()}",
            _ => $"{Name} {Independence.ToSourceCodeString()} {Variance.ToSourceCodeString()}"
        };
    }
}
