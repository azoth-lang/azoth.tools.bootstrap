using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class CodeNode : ChildNode, ICodeNode
{
    public abstract override ICodeSyntax? Syntax { get; }
    public virtual CodeFile File => Inherited_File(GrammarAttribute.CurrentInheritanceContext());

    private protected CodeNode() { }
}
