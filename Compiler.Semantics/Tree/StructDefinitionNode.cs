using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StructDefinitionNode : TypeDefinitionNode, IStructDefinitionNode
{
    public override IStructDefinitionSyntax Syntax { get; }
    private StructType? declaredType;
    private bool declaredTypeCached;
    public override StructType DeclaredType
        => GrammarAttribute.IsCached(in declaredTypeCached) ? declaredType!
            : this.Synthetic(ref declaredTypeCached, ref declaredType, TypeDeclarationsAspect.StructDefinition_DeclaredType);

    public IFixedList<IStructMemberDefinitionNode> SourceMembers { get; }
    private ValueAttribute<IFixedSet<IStructMemberDefinitionNode>> members;
    public override IFixedSet<IStructMemberDefinitionNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(this, DefaultMembersAspect.StructDefinition_Members);
    private ValueAttribute<IFixedSet<IStructMemberDeclarationNode>> inclusiveMembers;
    public override IFixedSet<IStructMemberDeclarationNode> InclusiveMembers
        => inclusiveMembers.TryGetValue(out var value) ? value
            : inclusiveMembers.GetValue(this, InheritanceAspect.StructDefinition_InclusiveMembers);
    private ValueAttribute<IDefaultInitializerDefinitionNode?> defaultInitializer;
    public IDefaultInitializerDefinitionNode? DefaultInitializer
        => defaultInitializer.TryGetValue(out var value) ? value
            : defaultInitializer.GetValue(this, DefaultMembersAspect.StructDefinition_DefaultInitializer);

    public StructDefinitionNode(
        IStructDefinitionSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IStructMemberDefinitionNode> sourceMembers)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        SourceMembers = ChildList.Attach(this, sourceMembers);
    }
}
