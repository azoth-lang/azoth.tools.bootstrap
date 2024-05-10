using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StructDeclarationNode : TypeDeclarationNode, IStructDeclarationNode
{
    public override IStructDeclarationSyntax Syntax { get; }

    private ValueAttribute<IStructDeclarationSymbolNode> symbolNode;
    public override IStructDeclarationSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value)
            ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.StructDeclaration);

    private ValueAttribute<StructType> declaredType;
    public override StructType DeclaredType
        => declaredType.TryGetValue(out var value) ? value
            : declaredType.GetValue(this, TypeDeclarationsAspect.StructDeclaration_DeclaredType);

    private ValueAttribute<CompilerResult<IFixedSet<BareReferenceType>>> supertypes;
    public override CompilerResult<IFixedSet<BareReferenceType>> Supertypes
        => supertypes.TryGetValue(out var value) ? value
            : supertypes.GetValue(this, TypeDeclarationsAspect.StructDeclaration_Supertypes);
    public override IFixedList<IStructMemberDeclarationNode> Members { get; }

    public StructDeclarationNode(
        IStructDeclarationSyntax syntax,
        IEnumerable<IGenericParameterNode> genericParameters,
        IEnumerable<IStandardTypeNameNode> supertypeNames,
        IEnumerable<IStructMemberDeclarationNode> members)
        : base(genericParameters, supertypeNames)
    {
        Syntax = syntax;
        Members = ChildList.CreateFixed(this, members);
    }
}
