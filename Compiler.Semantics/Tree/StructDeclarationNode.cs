using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StructDeclarationNode : TypeDeclarationNode, IStructDeclaration
{
    public override IStructDeclarationSyntax Syntax { get; }
    public override IFixedList<IStructMemberDeclaration> Members { get; }

    public StructDeclarationNode(
        IStructDeclarationSyntax syntax,
        IEnumerable<IGenericParameter> genericParameters,
        IEnumerable<ISupertypeName> supertypeNames,
        IEnumerable<IStructMemberDeclaration> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildList.CreateFixed(members);
    }
}
