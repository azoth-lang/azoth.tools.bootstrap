using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ReferenceCapabilityConstraintSyntax : Syntax, IReferenceCapabilityConstraintSyntax
{
    public ReferenceCapabilityConstraint Constraint { get; }

    public ReferenceCapabilityConstraintSyntax(TextSpan span, ReferenceCapabilityConstraint constraint)
        : base(span)
    {
        Constraint = constraint;
    }

    public override string ToString() => Constraint.ToString();
}
