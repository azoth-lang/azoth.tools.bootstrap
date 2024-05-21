using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class UnknownNameExpressionNode : AmbiguousNameExpressionNode, IUnknownNameExpressionNode
{
    public abstract override IStandardNameExpressionSyntax Syntax { get; }
    public abstract StandardName Name { get; }
    public UnknownType Type => (UnknownType)DataType.Unknown;
    public IFixedList<IDeclarationNode> ReferencedDeclarations { get; }

    private protected UnknownNameExpressionNode(IEnumerable<IDeclarationNode> referencedDeclarations)
    {
        ReferencedDeclarations = referencedDeclarations.ToFixedList();
    }
}
