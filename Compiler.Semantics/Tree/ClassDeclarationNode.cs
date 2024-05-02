using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ClassDeclarationNode : TypeDeclarationNode, IClassDeclaration
{
    public override IClassDeclarationSyntax Syntax { get; }
    public bool IsAbstract => Syntax.AbstractModifier is not null;
    public ISupertypeName? BaseTypeName { get; }
    public override IFixedList<IClassMemberDeclaration> Members { get; }

    public ClassDeclarationNode(
        IClassDeclarationSyntax syntax,
        IEnumerable<IGenericParameter> genericParameters,
        ISupertypeName? baseTypeName,
        IEnumerable<ISupertypeName> supertypeNames,
        IEnumerable<IClassMemberDeclaration> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        BaseTypeName = baseTypeName;
        Members = ChildList.CreateFixed(members);
    }
}
