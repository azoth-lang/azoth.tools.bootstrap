using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ClassDeclarationNode : TypeDeclarationNode, IClassDeclarationNode
{
    public override IClassDeclarationSyntax Syntax { get; }
    public bool IsAbstract => Syntax.AbstractModifier is not null;
    private Child<ISupertypeNameNode>? baseTypeName;
    public ISupertypeNameNode? BaseTypeName => baseTypeName?.Value;
    public override IFixedList<IClassMemberDeclarationNode> Members { get; }

    public ClassDeclarationNode(
        IClassDeclarationSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        ISupertypeNameNode? baseTypeName,
        IEnumerable<ISupertypeNameNode> supertypeNames,
        IEnumerable<IClassMemberDeclarationNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        this.baseTypeName = Child.Create(this, baseTypeName);
        Members = ChildList.CreateFixed(this, members);
    }
}
