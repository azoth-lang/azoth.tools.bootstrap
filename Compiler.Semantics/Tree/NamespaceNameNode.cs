using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class NamespaceNameNode : AmbiguousNameExpressionNode, INamespaceNameNode
{
    public abstract override INameExpressionSyntax Syntax { get; }
    public UnknownType Type => (UnknownType)DataType.Unknown;
    public abstract IFixedList<INamespaceDeclarationNode> ReferencedDeclarations { get; }

    private protected NamespaceNameNode() { }
}
