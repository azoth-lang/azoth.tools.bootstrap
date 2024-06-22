using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
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
    public override IFixedSet<IStructMemberDefinitionNode> Members { get; }
    private ValueAttribute<IFixedSet<IStructMemberDeclarationNode>> inclusiveMembers;
    public override IFixedSet<IStructMemberDeclarationNode> InclusiveMembers
        => inclusiveMembers.TryGetValue(out var value) ? value
            : inclusiveMembers.GetValue(this, InheritanceAspect.StructDefinition_InclusiveMembers);

    public StructDefinitionNode(
        IStructDefinitionSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IStructMemberDefinitionNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildSet.Attach(this, members);
    }
}
