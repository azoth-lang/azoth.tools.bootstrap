using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ControlFlowNode : ChildNode, IControlFlowNode
{
    public sealed override IConcreteSyntax? Syntax => null;
    public CodeFile File => InheritedFile();
    public abstract FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext { get; }

    public virtual FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowFollowing()
        => throw new System.NotImplementedException();
}
