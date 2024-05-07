using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ClassDeclarationNode : TypeDeclarationNode, IClassDeclarationNode
{
    public override IClassDeclarationSyntax Syntax { get; }
    public bool IsAbstract => Syntax.AbstractModifier is not null;
    private Child<ISupertypeNameNode>? baseTypeName;
    public ISupertypeNameNode? BaseTypeName => baseTypeName?.Value;

    private ValueAttribute<IClassSymbolNode> symbolNode;
    public override IClassSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value)
            ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.ClassDeclaration);

    private ValueAttribute<ObjectType> type;
    public ObjectType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeAttributes.Class);
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
        this.baseTypeName = Child.CreateOptional(this, baseTypeName);
        Members = ChildList.CreateFixed(this, members);
    }
}
