using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeMemberDefinitionNode : DefinitionNode, ITypeMemberDefinitionNode
{
    public abstract override ITypeMemberDefinitionSyntax? Syntax { get; }
    private ValueAttribute<AccessModifier> accessModifier;
    public AccessModifier AccessModifier
        => accessModifier.TryGetValue(out var value) ? value
            : accessModifier.GetValue(this, TypeModifiersAspect.TypeMemberDeclaration_AccessModifier);

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => this;
}
