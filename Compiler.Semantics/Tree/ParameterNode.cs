using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ParameterNode : CodeNode, IParameterNode
{
    public abstract override IParameterSyntax Syntax { get; }
    public abstract IdentifierName? Name { get; }
    public abstract Pseudotype Type { get; }
    public bool Unused => Syntax.Unused;

    private protected ParameterNode() { }
}
