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
    private Child<IStandardTypeNameNode>? baseTypeName;
    public IStandardTypeNameNode? BaseTypeName => baseTypeName?.Value;

    private ValueAttribute<IClassDeclarationSymbolNode> symbolNode;
    public override IClassDeclarationSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value)
            ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.ClassDeclaration);

    private ValueAttribute<ObjectType> declaredType;
    public override ObjectType DeclaredType
        => declaredType.TryGetValue(out var value) ? value
            : declaredType.GetValue(this, TypeDeclarationsAspect.ClassDeclaration_DeclaredType);

    public override IFixedList<IClassMemberDeclarationNode> Members { get; }

    public ClassDeclarationNode(
        IClassDeclarationSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IStandardTypeNameNode? baseTypeName,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IClassMemberDeclarationNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        this.baseTypeName = Child.CreateOptional(this, baseTypeName);
        Members = ChildList.CreateFixed(this, members);
    }
}
