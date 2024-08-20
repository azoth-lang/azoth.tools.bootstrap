using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class CodeNode : ChildNode, ICodeNode
{
    public abstract override ICodeSyntax? Syntax { get; }
    public virtual CodeFile File => InheritedFile();

    private protected CodeNode() { }
}
