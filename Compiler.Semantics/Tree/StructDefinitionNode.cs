using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StructDefinitionNode : TypeDefinitionNode, IStructDefinitionNode
{
    public override IStructDefinitionSyntax Syntax { get; }

    private ValueAttribute<IStructDeclarationNode> symbolNode;
    public override IStructDeclarationNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.StructDeclaration);

    private ValueAttribute<StructType> declaredType;
    public override StructType DeclaredType
        => declaredType.TryGetValue(out var value) ? value
            : declaredType.GetValue(this, TypeDeclarationsAspect.StructDeclaration_DeclaredType);

    public override IFixedList<IStructMemberDefinitionNode> Members { get; }

    public StructDefinitionNode(
        IStructDefinitionSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IStructMemberDefinitionNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildList.Attach(this, members);
    }
}
