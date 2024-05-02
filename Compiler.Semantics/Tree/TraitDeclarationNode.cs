using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class TraitDeclarationNode : TypeDeclarationNode, ITraitDeclarationNode
{
    public override ITraitDeclarationSyntax Syntax { get; }
    public override IFixedList<ITraitMemberDeclarationNode> Members { get; }

    public TraitDeclarationNode(
        ITraitDeclarationSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<ISupertypeNameNode> supertypeNames,
        IEnumerable<ITraitMemberDeclarationNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildList.CreateFixed(members);
    }
}
