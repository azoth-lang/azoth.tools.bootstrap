using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class GenericParameterSyntax : Syntax, IGenericParameterSyntax
{
    public ICapabilityConstraintSyntax Constraint { get; }
    public IdentifierName Name { get; }
    public ParameterIndependence Independence { get; }
    public ParameterVariance Variance { get; }
    public Promise<GenericParameterTypeSymbol> Symbol { get; } = new();

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
