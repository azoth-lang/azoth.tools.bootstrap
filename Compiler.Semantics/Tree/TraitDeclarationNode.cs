using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class TraitDeclarationNode : TypeDeclarationNode, ITraitDeclaration
{
    public override ITraitDeclarationSyntax Syntax { get; }
    public override IFixedList<ITraitMemberDeclaration> Members { get; }

    public TraitDeclarationNode(
        ITraitDeclarationSyntax syntax,
        IEnumerable<IGenericParameter> genericParameters,
        IEnumerable<ISupertypeName> supertypeNames,
        IEnumerable<ITraitMemberDeclaration> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildList.CreateFixed(members);
    }
}
