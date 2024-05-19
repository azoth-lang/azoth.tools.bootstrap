using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class CodeNode : ChildNode, ICodeNode
{
    public abstract override IConcreteSyntax? Syntax { get; }
    public virtual CodeFile File => InheritedFile();

    private protected CodeNode() { }
}
