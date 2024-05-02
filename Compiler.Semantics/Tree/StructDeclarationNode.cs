using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StructDeclarationNode : TypeDeclarationNode, IStructDeclarationNode
{
    public override IStructDeclarationSyntax Syntax { get; }
    public override IFixedList<IStructMemberDeclarationNode> Members { get; }

    public StructDeclarationNode(
        IStructDeclarationSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<ISupertypeNameNode> supertypeNames,
        IEnumerable<IStructMemberDeclarationNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildList.CreateFixed(this, members);
    }
}
