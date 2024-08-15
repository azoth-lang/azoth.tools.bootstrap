using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class GenericParameterSyntax : CodeSyntax, IGenericParameterSyntax
{
    public ICapabilityConstraintSyntax Constraint { get; }
    public IdentifierName Name { get; }
    public TypeParameterIndependence Independence { get; }
    public TypeParameterVariance Variance { get; }

    public GenericParameterSyntax(
        TextSpan span,
        ICapabilityConstraintSyntax constraint,
        IdentifierName name,
        TypeParameterIndependence independence,
        TypeParameterVariance variance)
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
            (TypeParameterIndependence.None, TypeParameterVariance.Invariant) => Name.ToString(),
            (TypeParameterIndependence.None, _) => $"{Name} {Variance.ToSourceCodeString()}",
            (_, TypeParameterVariance.Invariant) => $"{Name} {Independence.ToSourceCodeString()}",
            _ => $"{Name} {Independence.ToSourceCodeString()} {Variance.ToSourceCodeString()}"
        };
    }
}
