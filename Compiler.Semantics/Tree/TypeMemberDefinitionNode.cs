using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeMemberDefinitionNode : DefinitionNode, ITypeMemberDefinitionNode
{
    public abstract override ITypeMemberDeclarationSyntax Syntax { get; }
    private ValueAttribute<AccessModifier> accessModifier;
    public AccessModifier AccessModifier
        => accessModifier.TryGetValue(out var value) ? value
            : accessModifier.GetValue(this, TypeModifiersAspect.TypeMemberDeclaration_AccessModifier);
    public abstract override ITypeMemberSymbolNode SymbolNode { get; }
}
