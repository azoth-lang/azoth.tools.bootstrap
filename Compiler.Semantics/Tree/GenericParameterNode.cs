using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class GenericParameterNode : CodeNode, IGenericParameter
{
    public override IGenericParameterSyntax Syntax { get; }
    public ICapabilityConstraint Constraint { get; }
    public IdentifierName Name => Syntax.Name;
    public ParameterIndependence Independence => Syntax.Independence;
    public ParameterVariance Variance => Syntax.Variance;

    public GenericParameterNode(IGenericParameterSyntax syntax, ICapabilityConstraint constraint)
    {
        Syntax = syntax;
        Constraint = constraint;
    }
}
