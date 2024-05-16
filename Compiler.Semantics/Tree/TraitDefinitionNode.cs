using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class TraitDefinitionNode : TypeDefinitionNode, ITraitDefinitionNode
{
    public override ITraitDefinitionSyntax Syntax { get; }

    private ValueAttribute<ITraitDeclarationNode> symbolNode;
    public override ITraitDeclarationNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.TraitDeclaration);

    private ValueAttribute<ObjectType> declaredType;
    public override ObjectType DeclaredType
        => declaredType.TryGetValue(out var value) ? value
            : declaredType.GetValue(this, TypeDeclarationsAspect.TraitDeclaration_DeclaredType);

    public override IFixedList<ITraitMemberDefinitionNode> Members { get; }

    public TraitDefinitionNode(
        ITraitDefinitionSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<ITraitMemberDefinitionNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildList.Attach(this, members);
    }
}
