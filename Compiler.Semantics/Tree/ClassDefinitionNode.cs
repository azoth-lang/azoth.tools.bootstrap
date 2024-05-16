using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ClassDefinitionNode : TypeDefinitionNode, IClassDefinitionNode
{
    public override IClassDefinitionSyntax Syntax { get; }
    public bool IsAbstract => Syntax.AbstractModifier is not null;
    public IStandardTypeNameNode? BaseTypeName { get; }

    private ValueAttribute<ObjectType> declaredType;
    public override ObjectType DeclaredType
        => declaredType.TryGetValue(out var value) ? value
            : declaredType.GetValue(this, TypeDeclarationsAspect.ClassDeclaration_DeclaredType);

    public IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    private ValueAttribute<IFixedList<IClassMemberDefinitionNode>> members;
    public override IFixedList<IClassMemberDefinitionNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(this, DefaultMembersAspect.ClassDeclaration_Members);
    private ValueAttribute<IDefaultConstructorDefinitionNode?> defaultConstructor;
    public IDefaultConstructorDefinitionNode? DefaultConstructor
        => defaultConstructor.TryGetValue(out var value) ? value
            : defaultConstructor.GetValue(this, DefaultMembersAspect.ClassDeclaration_DefaultConstructor);

    public ClassDefinitionNode(
        IClassDefinitionSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IStandardTypeNameNode? baseTypeName,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IClassMemberDefinitionNode> sourceMembers)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        BaseTypeName = Child.Attach(this, baseTypeName);
        SourceMembers = ChildList.Attach(this, sourceMembers);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeDeclarationsAspect.ClassDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
