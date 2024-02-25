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
    public SimpleName Name { get; }
    public ParameterVariance ParameterVariance { get; }
    public Promise<GenericParameterTypeSymbol> Symbol { get; } = new();

    public GenericParameterSyntax(
        TextSpan span,
        ICapabilityConstraintSyntax constraint,
        SimpleName name,
        ParameterVariance variance)
        : base(span)
    {
        Constraint = constraint;
        Name = name;
        ParameterVariance = variance;
    }

    public override string ToString()
        => ParameterVariance == ParameterVariance.Invariant ? Name.ToString() : $"{ParameterVariance.ToSourceCodeString()} {Name}";
}
