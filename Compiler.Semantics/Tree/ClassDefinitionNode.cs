using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ClassDefinitionNode : TypeDefinitionNode, IClassDefinitionNode
{
    public override IClassDefinitionSyntax Syntax { get; }
    public bool IsAbstract => Syntax.AbstractModifier is not null;
    public IStandardTypeNameNode? BaseTypeName { get; }

    private ObjectType? declaredType;
    private bool declaredTypeCached;
    public override ObjectType DeclaredType
        => GrammarAttribute.IsCached(in declaredTypeCached) ? declaredType!
            : this.Synthetic(ref declaredTypeCached, ref declaredType, TypeDeclarationsAspect.ClassDefinition_DeclaredType);

    public IFixedList<IClassMemberDefinitionNode> SourceMembers { get; }
    private ValueAttribute<IFixedSet<IClassMemberDefinitionNode>> members;
    public override IFixedSet<IClassMemberDefinitionNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(this, DefaultMembersAspect.ClassDefinition_Members);
    private ValueAttribute<IFixedSet<IClassMemberDeclarationNode>> inclusiveMembers;
    public override IFixedSet<IClassMemberDeclarationNode> InclusiveMembers
        => inclusiveMembers.TryGetValue(out var value) ? value
            : inclusiveMembers.GetValue(this, InheritanceAspect.ClassDefinition_InclusiveMembers);
    private ValueAttribute<IDefaultConstructorDefinitionNode?> defaultConstructor;
    public IDefaultConstructorDefinitionNode? DefaultConstructor
        => defaultConstructor.TryGetValue(out var value) ? value
            : defaultConstructor.GetValue(this, DefaultMembersAspect.ClassDefinition_DefaultConstructor);

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

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        TypeDeclarationsAspect.ClassDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
