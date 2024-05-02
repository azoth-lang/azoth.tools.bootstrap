using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ClassDeclarationNode : TypeDeclarationNode, IClassDeclarationNode
{
    public override IClassDeclarationSyntax Syntax { get; }
    public bool IsAbstract => Syntax.AbstractModifier is not null;
    public ISupertypeNameNode? BaseTypeName { get; }
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
        BaseTypeName = baseTypeName;
        Members = ChildList.CreateFixed(members);
    }
}
