using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class TraitDefinitionNode : TypeDefinitionNode, ITraitDefinitionNode
{
    public override ITraitDefinitionSyntax Syntax { get; }
    private ObjectType? declaredType;
    private bool declaredTypeCached;
    public override ObjectType DeclaredType
        => GrammarAttribute.IsCached(in declaredTypeCached) ? declaredType!
            : this.Synthetic(ref declaredTypeCached, ref declaredType, TypeDefinitionsAspect.TraitDefinition_DeclaredType);
    public override IFixedSet<ITraitMemberDefinitionNode> Members { get; }
    private ValueAttribute<IFixedSet<ITraitMemberDeclarationNode>> inclusiveMembers;
    public override IFixedSet<ITraitMemberDeclarationNode> InclusiveMembers
        => inclusiveMembers.TryGetValue(out var value) ? value
            : inclusiveMembers.GetValue(this, InheritanceAspect.TraitDefinition_InclusiveMembers);

    public TraitDefinitionNode(
        ITraitDefinitionSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<ITraitMemberDefinitionNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildSet.Attach(this, members);
    }
}
