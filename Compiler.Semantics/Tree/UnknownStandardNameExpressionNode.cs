using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class UnknownStandardNameExpressionNode : UnknownNameExpressionNode, IUnknownStandardNameExpressionNode
{
    public abstract override IStandardNameExpressionSyntax Syntax { get; }
    public abstract StandardName Name { get; }
    public IFixedSet<IDeclarationNode> ReferencedDeclarations { get; }

    private protected UnknownStandardNameExpressionNode(IEnumerable<IDeclarationNode> referencedDeclarations)

    {
        ReferencedDeclarations = referencedDeclarations.ToFixedSet();
    }
}
