using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ParameterNode : CodeNode, IParameterNode
{
    public abstract override IParameterSyntax Syntax { get; }
    public abstract IdentifierName? Name { get; }
    public bool Unused => Syntax.Unused;
    public abstract Pseudotype Type { get; }

    private ValueAttribute<ValueId> valueId;
    public ValueId ValueId
        => valueId.TryGetValue(out var value) ? value
            : valueId.GetValue(this, TypeMemberDeclarationsAspect.Parameter_ValueId);

    private protected ParameterNode() { }

    public IParameterNode? Predecessor() => (IParameterNode?)InheritedPredecessor();
}
